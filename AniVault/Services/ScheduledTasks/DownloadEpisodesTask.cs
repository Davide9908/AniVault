using AniVault.Database;
using AniVault.Database.Context;
using AniVault.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using TL;

namespace AniVault.Services.ScheduledTasks;

public class DownloadEpisodesTask : BaseTask
{
    private const int MaxConcurrentDownload = 2;
    private const string DownloadingPrefix = "downloading_";

    private readonly ILogger<DownloadEpisodesTask> _log;
    private readonly AniVaultDbContext _dbContext;
    private readonly TelegramClientService _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _defaultDownloadLocation;
    private readonly bool _converterEnabled;
    
    public DownloadEpisodesTask(ILogger<DownloadEpisodesTask> log, AniVaultDbContext dbContext, TelegramClientService client, IServiceProvider serviceProvider, IConfiguration configuration) : base(log, dbContext)
    {
        _log = log;
        _dbContext = dbContext;
        _client = client;
        _serviceProvider = serviceProvider;
        _defaultDownloadLocation = configuration["DefaultDownloadLocation"] ?? throw new InvalidOperationException("DefaultDownloadLocation configuration missing");
        _converterEnabled = configuration.GetValue<bool>("AniVaultConverterEnabled");
    }

    protected override Task Run()
    {
        List<TelegramMediaDocument> episodesToDownload = _dbContext.TelegramMediaDocuments.Where(md =>
                md.AnimeConfiguration.AutoDownloadEnabled 
                && md.TelegramMessage.TelegramChannel.Status == ChannelStatus.Active
                && md.TelegramMessage.TelegramChannel.IsAnimeChannel
                && md.DownloadStatus == DownloadStatus.NotStarted
            )
            .Take(MaxConcurrentDownload)
            .ToList();
        foreach (TelegramMediaDocument episode in episodesToDownload)
        {
            string filePath;
            if (_converterEnabled)
            {
                filePath = Path.Combine(_defaultDownloadLocation, "Downloading", DownloadingPrefix + episode.Filename);
            }
            else
            {
                filePath = Path.Combine(_defaultDownloadLocation, DownloadingPrefix + episode.Filename);
            }
            FileStream? fileStream;
            try
            {
                fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "An error occured while creating the file {filePath}", filePath);
                continue;
            }
            episode.DownloadStatus = DownloadStatus.Downloading;
            _dbContext.SaveChangesAsync(_ct);
            _ = DownloadEpisode(fileStream, episode, filePath);
        }

        return Task.CompletedTask;
    }
    
    private async Task DownloadEpisode(FileStream fileStream, TelegramMediaDocument dbFile, string filePath)
    {
        using var downScope = _serviceProvider.CreateScope();
        var log = downScope.ServiceProvider.GetRequiredService<ILogger<DownloadEpisodesTask>>();
        try
        {
            string path = fileStream.Name;
            await using var downDbContext = downScope.ServiceProvider.GetRequiredService<AniVaultDbContext>();
            var downDbFile =
                downDbContext.TelegramMediaDocuments
                    .Include(f=>f.TelegramMessage.TelegramChannel)
                    .First(md =>
                    md.TelegramMediaDocumentId == dbFile.TelegramMediaDocumentId);
            Document doc = new Document()
            {
                id = downDbFile.FileId,
                access_hash = downDbFile.AccessHash,
                file_reference = downDbFile.FileReference
            };
            int retry = 1;
            while (true)
            {
                try
                {
                    int time = Random.Shared.Next(3000, 11000);
                    await Task.Delay(time);
                    ProgressState progressState = new ProgressState();
                    await _client.DownloadFileAsync(doc, fileStream,
                        (transmitted, totalSize) =>
                        {
                            DownloadProgressCallback(transmitted, totalSize, downDbFile, progressState, downDbContext,
                                log);
                        }, retry > 1); //if it's the second try (after a FILE_REFERENCE_EXPIRED exception), dispose, otherwise let me handle it
                }
                catch (RpcException rpcEx)
                    when (rpcEx.Code == 400 && rpcEx.Message.Contains("FILE_REFERENCE_EXPIRED"))
                {
                    if (retry > 1)
                    {
                        log.Error(
                            "FILE_REFERENCE_EXPIRED got from downloading, but I'm already retrying a second time, aborting download. file {filename} to {fileNamePath}",
                            downDbFile.FilenameFromTelegram, fileStream.Name);
                        await HandleDownloadError(path, downDbFile, downDbContext, fileStream, log);
                        return;
                    }
                    Messages_MessagesBase? messages = await _client.GetChannelMessagesByIds(
                        downDbFile.TelegramMessage.TelegramChannel.ChatId,
                        downDbFile.TelegramMessage.TelegramChannel.AccessHash, [downDbFile.TelegramMessage.MessageId]);
                    if (messages is null || messages.Messages.Length == 0)
                    {
                        log.Error(
                            "FILE_REFERENCE_EXPIRED got from downloading, but TG did not returned the message by ID. file {filename} to {fileNamePath}",
                            downDbFile.FilenameFromTelegram, fileStream.Name);

                        await HandleDownloadError(path, downDbFile, downDbContext, fileStream, log);
                        return;
                    }

                    byte[]? newFileRef;
                    if ((newFileRef =
                            (((messages.Messages[0] as Message)?.media as MessageMediaDocument)?.document as Document)
                            ?.file_reference) is null)
                    {
                        log.Error(
                            "FILE_REFERENCE_EXPIRED got from downloading, but the message from TG seems to not have a file or a file_reference. file {filename} to {fileNamePath}",
                            downDbFile.FilenameFromTelegram, fileStream.Name);

                        await HandleDownloadError(path, downDbFile, downDbContext, fileStream, log);
                        return;
                    }

                    downDbFile.FileReference = newFileRef;
                    downDbContext.SaveChanges();
                    retry++;
                    continue;
                }
                catch (Exception ex)
                {
                    log.Error(ex, "An error occured downloading file {filename} to {fileNamePath}",
                        downDbFile.FilenameFromTelegram, fileStream.Name);

                    await HandleDownloadError(path, downDbFile, downDbContext, fileStream, log);

                    return;
                }

                downDbFile.DownloadStatus = DownloadStatus.Completed;
                downDbFile.LastUpdateDateTime = DateTime.UtcNow;

                await downDbContext.SaveChangesAsync();
                await fileStream.DisposeAsync();
                log.Info("Download of file {filename} to {fileNamePath} completed", downDbFile.FilenameFromTelegram,
                    fileStream.Name);

                string newPath = path.Replace(DownloadingPrefix, String.Empty);
                File.Move(path, newPath);
                log.Info("File {oldFilePath} renamed to {newFilePath}", path, newPath);
                
                break; //exit the loop
            }
        }
        catch (Exception ex)
        {
            log.Error(ex, "Error occured on DownloadEpisode");
        }
    }
    
    private void DownloadProgressCallback(long transmitted, long totalSize, TelegramMediaDocument dbFile, ProgressState progressState, AniVaultDbContext downDbContext, ILogger<DownloadEpisodesTask> log)
    {

        dbFile.DataTransmitted = transmitted;
        dbFile.LastUpdateDateTime = DateTime.UtcNow;
        downDbContext.SaveChanges();
        if (totalSize == 0)
        {
            if (dbFile.Size == 0)
            {
                _log.Warning("Total size is 0, cannot calculate download progress percentage");
                return;
            }
            totalSize = dbFile.Size;
        }
        decimal percentage = decimal.Divide(transmitted, totalSize) * 100;
        if (percentage - progressState.OldPercentage > Convert.ToDecimal(1.5))
        {
            progressState.OldPercentage = percentage;
            log.Info("{percentage}% - Downloading file {fileName}: {transmitted}/{totalSize}", percentage, dbFile.Filename, transmitted, totalSize);
        }

    }

    private async Task HandleDownloadError(string path, TelegramMediaDocument downDbFile, AniVaultDbContext downDbContext, FileStream fileStream, ILogger<DownloadEpisodesTask> log)
    {
        downDbFile.DownloadStatus = DownloadStatus.Error;

        downDbFile.LastUpdateDateTime = DateTime.UtcNow;
        await downDbContext.SaveChangesAsync();
        await fileStream.DisposeAsync();

        try
        {
            File.Delete(path);
        }
        catch (Exception deleteException)
        {
            log.Error(deleteException, "Unable to delete file for failed download");
        }
    }
    private class ProgressState
    {
        public decimal OldPercentage { get; set; } = decimal.Zero;
    }
}
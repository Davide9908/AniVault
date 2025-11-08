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
    private readonly string _defaultDownloadLocation;
    private readonly bool _converterEnabled;
    
    public DownloadEpisodesTask(ILogger<DownloadEpisodesTask> log, AniVaultDbContext dbContext, TelegramClientService client, IConfiguration configuration) : base(log, dbContext)
    {
        _log = log;
        _dbContext = dbContext;
        _client = client;
        _defaultDownloadLocation = configuration["DefaultDownloadLocation"] ?? throw new InvalidOperationException("DefaultDownloadLocation configuration missing");
        _converterEnabled = configuration.GetValue<bool>("AniVaultConverterEnabled");
    }

    protected override async Task Run()
    {
        int downloadOngoingCount = _dbContext.TelegramMediaDocuments.Count(md => md.DownloadStatus == DownloadStatus.Downloading);
        if (downloadOngoingCount >= MaxConcurrentDownload)
        {
            return;
        }
        TelegramMediaDocument? episodeToDownload = _dbContext.TelegramMediaDocuments
            .Include(md => md.TelegramMessage.TelegramChannel)
            .FirstOrDefault(md =>
                md.AnimeConfiguration.AutoDownloadEnabled 
                && md.TelegramMessage.TelegramChannel.Status == ChannelStatus.Active
                && md.TelegramMessage.TelegramChannel.IsAnimeChannel
                && md.DownloadStatus == DownloadStatus.NotStarted);
        if (episodeToDownload is null)
        {
            return;
        }
        
        string filePath;
        if (_converterEnabled)
        {
            filePath = Path.Combine(_defaultDownloadLocation, "Downloading", DownloadingPrefix + episodeToDownload.Filename);
        }
        else
        {
            filePath = Path.Combine(_defaultDownloadLocation, DownloadingPrefix + episodeToDownload.Filename);
        }
        FileStream? fileStream;
        try
        {
            fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "An error occured while creating the file {filePath}", filePath);
            return;
        }
        episodeToDownload.DownloadStatus = DownloadStatus.Downloading;
        await _dbContext.SaveChangesAsync(_ct);
        await DownloadEpisode(fileStream, episodeToDownload, filePath);
    }
    
    private async Task DownloadEpisode(FileStream fileStream, TelegramMediaDocument dbFile, string filePath)
    {
        try
        {
            Document doc = new Document
            {
                id = dbFile.FileId,
                access_hash = dbFile.AccessHash,
                file_reference = dbFile.FileReference
            };
            
            int retry = 1;
            int retryTimeout = 1;
            bool completed = false;
            
            while (retryTimeout < 4 && !completed)
            {
                try
                {
                    ProgressState progressState = new ProgressState();
                    await _client.DownloadFileAsync(doc, fileStream,
                        (transmitted, totalSize) =>
                        {
                            DownloadProgressCallback(transmitted, totalSize, dbFile, progressState);
                        }, retry > 1); //if it's the second try (after a FILE_REFERENCE_EXPIRED exception), dispose, otherwise let me handle it
                    
                    completed = true; //exit the loop
                }
                catch (RpcException rpcEx)
                    when (rpcEx.Code == 400 && rpcEx.Message.Contains("FILE_REFERENCE_EXPIRED"))
                {
                    if (await HandleFileReferenceExpired(fileStream, dbFile, retry, filePath, doc))
                    {
                        return;
                    }
                    retry++;
                    
                    await Task.Delay(1000);
                    continue;
                }
                catch (RpcException rpcEx)
                    when (rpcEx.Code == -503 && rpcEx.Message.Contains("Timeout"))
                {
                    await HandleDownloadTimeoutError(filePath, dbFile, fileStream);
                    _log.Error(rpcEx, "Timeout error, retrying download. file {filename} to {fileNamePath}", dbFile.FilenameFromTelegram, fileStream.Name);
                    try
                    {
                        fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, "An error occured while creating the file {filePath}", filePath);
                        return;
                    }
                    retryTimeout = 4;
                    await Task.Delay(2000);
                    continue;
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "An error occured downloading file {filename} to {fileNamePath}",
                        dbFile.FilenameFromTelegram, fileStream.Name);

                    await HandleDownloadError(filePath, dbFile, fileStream);

                    return;
                }

                dbFile.DownloadStatus = DownloadStatus.Completed;
                dbFile.LastUpdateDateTime = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                await fileStream.DisposeAsync();
                _log.Info("Download of file {filename} to {fileNamePath} completed", dbFile.FilenameFromTelegram,
                    fileStream.Name);

                string newPath = filePath.Replace(DownloadingPrefix, String.Empty);
                File.Move(filePath, newPath);
                _log.Info("File {oldFilePath} renamed to {newFilePath}", filePath, newPath);
                
            }

            if (retryTimeout == 4)
            {
                _log.Error("Download in timeout, aborting download");
                await HandleDownloadError(filePath, dbFile, fileStream);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error occured on DownloadEpisode");
            await HandleDownloadError(filePath, dbFile, fileStream);
        }
    }

    private void DownloadProgressCallback(long transmitted, long totalSize, TelegramMediaDocument dbFile, ProgressState progressState)
    {
        CancellationToken.ThrowIfCancellationRequested();
        dbFile.DataTransmitted = transmitted;
        dbFile.LastUpdateDateTime = DateTime.UtcNow;
        _dbContext.SaveChanges();
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
            _log.Info("{percentage:0.00}% - Downloading file {fileName}: {transmitted}/{totalSize}", percentage, dbFile.Filename, transmitted, totalSize);
        }

    }

    private async Task HandleDownloadError(string path, TelegramMediaDocument downDbFile, FileStream fileStream)
    {
        downDbFile.DownloadStatus = DownloadStatus.Error;

        downDbFile.LastUpdateDateTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        await fileStream.DisposeAsync();

        try
        {
            File.Delete(path);
        }
        catch (Exception deleteException)
        {
            _log.Error(deleteException, "Unable to delete file for failed download");
        }
    }
    
    private async Task HandleDownloadTimeoutError(string path, TelegramMediaDocument downDbFile, FileStream fileStream)
    {
        downDbFile.LastUpdateDateTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        await fileStream.DisposeAsync();

        try
        {
            File.Delete(path);
        }
        catch (Exception deleteException)
        {
            _log.Error(deleteException, "Unable to delete file for failed download");
        }
    }

    private async ValueTask<bool> HandleFileReferenceExpired(FileStream fileStream, TelegramMediaDocument dbFile, int retry,
        string path, Document doc)
    {
        if (retry > 1)
        {
            _log.Error(
                "FILE_REFERENCE_EXPIRED got from downloading, but I'm already retrying a second time, aborting download. file {filename} to {fileNamePath}",
                dbFile.FilenameFromTelegram, fileStream.Name);
            await HandleDownloadError(path, dbFile, fileStream);
            return true;
        }

        Messages_MessagesBase? messages = await _client.GetChannelMessagesByIds(
            dbFile.TelegramMessage.TelegramChannel.ChatId,
            dbFile.TelegramMessage.TelegramChannel.AccessHash, [dbFile.TelegramMessage.MessageId]);
        if (messages is null || messages.Messages.Length == 0)
        {
            _log.Error(
                "FILE_REFERENCE_EXPIRED got from downloading, but TG did not returned the message by ID. file {filename} to {fileNamePath}",
                dbFile.FilenameFromTelegram, fileStream.Name);

            await HandleDownloadError(path, dbFile, fileStream);
            return true;
        }

        byte[]? newFileRef;
        if ((newFileRef =
                (((messages.Messages[0] as Message)?.media as MessageMediaDocument)?.document as Document)
                ?.file_reference) is null)
        {
            _log.Error(
                "FILE_REFERENCE_EXPIRED got from downloading, but the message from TG seems to not have a file or a file_reference. file {filename} to {fileNamePath}",
                dbFile.FilenameFromTelegram, fileStream.Name);

            await HandleDownloadError(path, dbFile, fileStream);
            return true;
        }

        dbFile.FileReference = newFileRef;
        doc.file_reference = newFileRef;
        await _dbContext.SaveChangesAsync();
        return false;
    }
    
    private class ProgressState
    {
        public decimal OldPercentage { get; set; } = decimal.Zero;
    }
}
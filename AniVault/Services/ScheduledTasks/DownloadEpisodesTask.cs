using AniVault.Database;
using AniVault.Database.Context;
using AniVault.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using TL;

namespace AniVault.Services.ScheduledTasks;

public class DownloadEpisodesTask : BaseTask
{
    private const int MaxConcurrentDownload = 2;
    
    private readonly ILogger<DownloadEpisodesTask> _log;
    private readonly AniVaultDbContext _dbContext;
    private readonly TelegramClientService _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _defaultDownloadLocation;
    
    public DownloadEpisodesTask(ILogger<DownloadEpisodesTask> log, AniVaultDbContext dbContext, TelegramClientService client, IServiceProvider serviceProvider, IConfiguration configuration) : base(log, dbContext)
    {
        _log = log;
        _dbContext = dbContext;
        _client = client;
        _serviceProvider = serviceProvider;
        _defaultDownloadLocation = configuration["DefaultDownloadLocation"] ?? throw new InvalidOperationException("DefaultDownloadLocation configuration missing");
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
            string filePath = Path.Combine(_defaultDownloadLocation, episode.Filename);
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
            _ = DownloadEpisode(fileStream, episode);
        }

        return Task.CompletedTask;
    }
    
    private async Task DownloadEpisode(FileStream fileStream, TelegramMediaDocument dbFile)
    {
        string path = fileStream.Name;
        
        using var downScope = _serviceProvider.CreateScope();
        await using var downDbContext = downScope.ServiceProvider.GetRequiredService<AniVaultDbContext>();
        var log = downScope.ServiceProvider.GetRequiredService<ILogger<DownloadEpisodesTask>>();
        
        var downDbFile = downDbContext.TelegramMediaDocuments.First(md => md.TelegramMediaDocumentId == dbFile.TelegramMediaDocumentId);
        Document doc = new Document()
        {
            id = downDbFile.FileId,
            access_hash = downDbFile.AccessHash,
            file_reference = downDbFile.FileReference
        };
        try
        {
            int time = Random.Shared.Next(3000, 11000);
            await Task.Delay(time);
            ProgressState progressState = new ProgressState();
            await _client.DownloadFileAsync(doc, fileStream, (transmitted, totalSize) => { DownloadProgressCallback(transmitted, totalSize, downDbFile, progressState,downDbContext, log); });
        }
        catch (Exception ex)
        {
            log.Error(ex, "An error occured downloading file {filename} to {fileNamePath}", downDbFile.FilenameFromTelegram, fileStream.Name);

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
            return;
        }

        downDbFile.DownloadStatus = DownloadStatus.Completed;
        downDbFile.LastUpdateDateTime = DateTime.UtcNow;
        
        await downDbContext.SaveChangesAsync();
        await fileStream.DisposeAsync();
        log.Info("Download of file {filename} to {fileNamePath} completed", downDbFile.FilenameFromTelegram, fileStream.Name);
    }
    
    private void DownloadProgressCallback(long transmitted, long totalSize, TelegramMediaDocument dbFile, ProgressState progressState, AniVaultDbContext downDbContext, ILogger<DownloadEpisodesTask> log)
    {

        dbFile.DataTransmitted = transmitted;
        dbFile.LastUpdateDateTime = DateTime.UtcNow;
        downDbContext.SaveChanges();
        decimal percentage = decimal.Divide(transmitted, totalSize) * 100;
        if (percentage - progressState.OldPercentage > Convert.ToDecimal(1.5))
        {
            progressState.OldPercentage = percentage;
            log.Info("{percentage}%} - Downloading file {fileName}: {transmitted}/{totalSize}", percentage, dbFile.Filename, transmitted, totalSize);
        }

    }
    private class ProgressState
    {
        public decimal OldPercentage { get; set; } = decimal.Zero;
    }
}
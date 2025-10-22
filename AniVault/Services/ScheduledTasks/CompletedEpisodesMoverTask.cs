using AniVault.Database;
using AniVault.Database.Context;
using AniVault.Services.Classes;
using AniVault.Services.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AniVault.Services.ScheduledTasks;

public class CompletedEpisodesMoverTask : TransactionalTask
{
    private readonly ILogger<CompletedEpisodesMoverTask> _log;
    private readonly AniVaultDbContext _dbContext;
    private readonly MalApiHttpClient _malApiClient;
    private readonly string _defaultDownloadLocation;
    private readonly string _libraryPath;
    public CompletedEpisodesMoverTask(ILogger<CompletedEpisodesMoverTask> log, AniVaultDbContext context, IServiceScopeFactory scopeFactory, MalApiHttpClient apiClient, IConfiguration configuration) : base(log, context, scopeFactory)
    {
        _log = log;
        _dbContext = context;
        _malApiClient = apiClient;
        
        _defaultDownloadLocation = configuration["DefaultDownloadLocation"] ?? throw new InvalidOperationException("DefaultDownloadLocation configuration missing");
        _libraryPath = configuration["LibraryPath"] ?? throw new InvalidOperationException("LibraryPath configuration missing");;
    }

    protected sealed override async Task Run()
    {
        string[]? fileNames = GetDownloadedFileNames(_defaultDownloadLocation);

        if (fileNames is null || fileNames.Length == 0)
        {
            return;
        }

        List<MALAnimeData>? animeWatchingList = await _malApiClient.GetWatchingAnimeList();
        List<MALAnimeData>? animeCompletedList = await _malApiClient.GetCompletedAnimeList();
        if (animeWatchingList is null || animeCompletedList is null)
        {
            _log.Error("Returned anime lists is null. No work will be done");
            return;
        }


        var dbFiles = _dbContext.TelegramMediaDocuments.Where(md => md.DownloadStatus == DownloadStatus.Completed && fileNames.Contains(md.Filename))
                                                                                    .Include(md=>md.AnimeConfiguration)
                                                                                    .ToDictionary(md => md.Filename, md => md.AnimeConfiguration);

        foreach (var filename in fileNames)
        {
            if (!dbFiles.ContainsKey(filename))
            {
                continue;
            }

            AnimeConfiguration setting = dbFiles.GetValueOrDefault(filename, new AnimeConfiguration());
            if (setting?.AnimeFolderRelativePath is null || setting?.MyAnimeListId is null)
            {
                _log.Warning("AnimeConfiguration folder or MAL id are not configured");
                continue;
            }
            if (string.IsNullOrWhiteSpace(setting.FileNameTemplate))
            {
                _log.Warning("Filename template not configured, skipping...");
                continue;
            }
            string? epNumberString = filename.Replace(setting.FileNameTemplate, string.Empty).Split(".").FirstOrDefault();
            if (string.IsNullOrWhiteSpace(epNumberString))
            {
                _log.Error("Could not extract ep number from file name");
                continue;
            }
            int epNumber;
            try
            {
                epNumber = int.Parse(epNumberString);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Could not parse episode number from file name");
                continue;
            }
            if (setting.EpisodesNumberOffset is > 0)
            {
                epNumber -= setting.EpisodesNumberOffset.Value;
            }
            //I look first on watching list. If it's not present, i look into the completed ones
            var animeEntry = animeWatchingList.FirstOrDefault(l => l.node.id == setting.MyAnimeListId)?.list_status;
            animeEntry ??= animeCompletedList.FirstOrDefault(l => l.node.id == setting.MyAnimeListId)?.list_status;
            //run the check again to see if i found it
            if (animeEntry is null)
            {
                _log.Warning("Anime with id {malId} not found in MAL", setting.MyAnimeListId);
                continue;
            }
            if (animeEntry.num_episodes_watched >= epNumber)
            {
                string fileWithPath = Path.Combine(_defaultDownloadLocation, filename);
                string destination = Path.Combine(_libraryPath, setting.AnimeFolderRelativePath, filename);
                File.Move(fileWithPath, destination);
            }
        }
    }
    
    private string[]? GetDownloadedFileNames(string downloadFolder)
    {
        string[] files;
        try
        {
            files = Directory.GetFiles(downloadFolder).Select(Path.GetFileName).ToArray();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "An error occurred retrieving files from download path");
            return null;
        }
        return files;
    }
}
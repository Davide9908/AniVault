using System.Text.RegularExpressions;
using AniVault.Database;
using AniVault.Database.Context;

namespace AniVault.Services.Classes;

public class AnimeEpisodeService
{
    private readonly ILogger<AnimeEpisodeService> _log;
    private readonly AniVaultDbContext _dbContext;

    public AnimeEpisodeService(ILogger<AnimeEpisodeService> log, AniVaultDbContext dbContext)
    {
        _log = log;
        _dbContext = dbContext;
    }
    public string GetAnimeNameFromMessageText(string messageText)
    {
        return messageText.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)[0];
    }
    
    public string? GetEpNumberFromMessageText(string messageText)
    {
        string seasonEpisodeLine = messageText.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)[1];
        Match match = RegexUtils.EpRegex().Match(seasonEpisodeLine);
        if (!match.Success)
        {
            return null;
        }

        int indexE = match.Value.IndexOf('E', StringComparison.InvariantCultureIgnoreCase);
        return match.Value.Substring(indexE, match.Value.Length - indexE - 1); //for example the string S01E03 has lenght 6 but the 'E' is on index 3 and i need to take only '0' and '3', so i need to take lenght - the index found - 1, otherwise it would try to take 3 characters and it would throw ArgumentOutOfRangeException 
    }

    public List<AnimeConfiguration> GetAllAnimeConfigurations()
    {
        return _dbContext.AnimeConfigurations.ToList();
    }
}
using AniVault.Database;
using AniVault.Services.Extensions;

namespace AniVault.Services.Classes;

public class MalApiHttpClientService
{
    private readonly ILogger<MalApiHttpClientService> _log;
    private readonly HttpClient _httpClient;
    private readonly string _malUsername;

    private const string MalWatching = "watching";
    private const string MalCompleted = "completed";
    private const string ParameterLimit = "limit";
    private const int ParameterLimitValue = 1000;
    private const string ParameterFields = "fields";
    private const string ParameterFieldsValue = "list_status";
    private const string ParameterStatus = "status";
    private const string ParameterNsfw = "nsfw";
    public MalApiHttpClientService(ILogger<MalApiHttpClientService> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _log = logger;
        _httpClient = httpClient;
        _malUsername = configuration.GetRequiredSection("MalApiSettings").GetValue<string>("MALUsername") ?? throw new InvalidOperationException("MalApiSettings : MALUsername not set");
    }

    /// <summary>
    /// Get watching anime list from MyAnimeList 
    /// </summary>
    /// <returns>null if needed parameters is missing or request failed, otherwise the list of anime with status "watching"</returns>
    public async Task<List<MALAnimeData>?> GetWatchingAnimeList()
    {
        return await GetAnimeListByStatus(MalWatching);
    }

    /// <summary>
    /// Get completed anime list from MyAnimeList 
    /// </summary>
    /// <returns>null if needed parameters is missing or request failed, otherwise the list of anime with status "completed"</returns>
    public async Task<List<MALAnimeData>?> GetCompletedAnimeList()
    {
        return await GetAnimeListByStatus(MalCompleted);
    }

    private async Task<List<MALAnimeData>?> GetAnimeListByStatus(string status, bool includeNSFW = true)
    {
        string relativeUri = $"users/{_malUsername}/animelist?{ParameterFields}={ParameterFieldsValue}&{ParameterLimit}={ParameterLimitValue}&{ParameterStatus}={status}&{ParameterNsfw}={includeNSFW.ToString().ToLower()}";
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Root>(relativeUri);
            return response?.data ?? [];
        }
        catch (Exception ex)
        {
            _log.Error(ex, "An error occured while getting anime list");
            return null;
        }
        
    }

}


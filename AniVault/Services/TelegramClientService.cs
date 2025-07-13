using System.Collections.Concurrent;
using System.Text;
using AniVault.Database;
using AniVault.Services.Classes;
using AniVault.Services.Extensions;
using TL;
using WTelegram;

namespace AniVault.Services;

    public partial class TelegramClientService : IAsyncDisposable
{
    private readonly ILogger<TelegramClientService> _log;
    private readonly IServiceProvider _serviceProvider;
    private readonly TGAuthenticationSettings _configuration;

    private Client? _tgClient;
    private readonly SemaphoreSlim _semaphoreConnect;
    private readonly SemaphoreSlim _semaphoreDisconnect;
    private const string UpdateFile = "updates.save";

    private UpdateManager _updateManager = null!;

    private readonly StreamWriter? _wTelegramLogs;


    public TelegramClientService(ILogger<TelegramClientService> log, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _log = log;
        _configuration = configuration.GetSection("TGAuthenticationSettings").Get<TGAuthenticationSettings>() ??
                         throw new ApplicationException("Telegram authentication settings not found");
        var logPath = configuration["WTelegramClientLogPath"];
        if (!string.IsNullOrWhiteSpace(logPath))
        {
            _wTelegramLogs = new StreamWriter(logPath, true, Encoding.UTF8) { AutoFlush = true };
            Helpers.Log += (lvl, str) => _wTelegramLogs.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{"TDIWE!"[lvl]}] {str}");
        }
        _semaphoreConnect = new SemaphoreSlim(1);
        _semaphoreDisconnect = new SemaphoreSlim(1);
        
        InitializeNewClient();
    }
    
    private void InitializeNewClient()
    {
        _tgClient = new Client(ClientConfig);
        _tgClient.MaxAutoReconnects = 0;
    }
    
    private string? ClientConfig(string what)
    {
        switch (what)
        {
            case "api_id": return _configuration.ApiId;
            case "api_hash": return _configuration.ApiHash;
            case "session_pathname": return _configuration.SessionPath;
            case "phone_number": return _configuration.PhoneNumber;
            case "verification_code": Console.Write("Code: "); return Console.ReadLine();
            case "password": return _configuration.Password;
            default: return null; // let WTelegramClient decide the default config
        }
    }
    

    public async Task<User?> Connect(bool throwIfError = false)
    {
        User? loggedUser = null;
        //Evito che piu task possano effettuare il login
        try
        {
            await _semaphoreConnect.WaitAsync();
            if (_tgClient is not null && !_tgClient.Disconnected && _tgClient.User is not null)
            {
                return _tgClient.User;
            }

            _tgClient = new Client(ClientConfig);
            _tgClient.MaxAutoReconnects = 0;
            _updateManager = _tgClient.WithUpdateManager(Client_OnUpdate, UpdateFile);

            //_tgClient.OnUpdates += Client_OnUpdate;
            _tgClient.OnOther += Client_OnOther;

            await _tgClient.ConnectAsync();
            loggedUser = await _tgClient.LoginUserIfNeeded();

            var dialogs = await _tgClient.Messages_GetAllDialogs(); // dialogs = groups/channels/users
            dialogs.CollectUsersChats(_updateManager.Users, _updateManager.Chats);
        }
        catch (Exception ex)
        {
            if (throwIfError) { throw; }
            _log.Error(ex,"Unable to login or connect to Telegram");
        }
        finally
        {
            _semaphoreConnect.Release();
        }

        return loggedUser;
    }

    private async Task DisconnectAndClear()
    {
        await _semaphoreDisconnect.WaitAsync();
        if (_tgClient is null)
        {
            return;
        }
        _updateManager.SaveState(UpdateFile);
        _tgClient.OnOther -= Client_OnOther;
        await _tgClient.DisposeAsync();
        _tgClient = null;
        _log.Info("Telegram client has been disconnected");
        
        _semaphoreDisconnect.Release();
    }


    public async ValueTask DisposeAsync()
    {
        await DisconnectAndClear();
        _semaphoreConnect.Dispose();
        _semaphoreDisconnect.Dispose();
        if (_wTelegramLogs is not null)
        {
            await _wTelegramLogs.DisposeAsync();
        }

    }

}

public record ChannelFileUpdate
{
    public Channel Channel { get; set; }
    public Message Message { get; set; }
    public bool SusChannel { get; set; }

    public ChannelFileUpdate(Channel channel, Message message, bool isSus)
    {
        Channel = channel;
        Message = message;
        SusChannel = isSus;
    }
}

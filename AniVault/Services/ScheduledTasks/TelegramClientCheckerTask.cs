using System.Net.Sockets;
using AniVault.Services.Extensions;

namespace AniVault.Services.ScheduledTasks;

public class TelegramClientCheckerTask : BaseTask
{
    private readonly ILogger<TelegramClientCheckerTask> _log;
    private readonly TelegramClientService _tgClient;
    
    //TimeSpan's milliseconds are 10000 ticks, so i have to divide it in order to compare it correctly with Environment.TickCount64
    private const long TimeoutTicks = TimeSpan.TicksPerMinute / TimeSpan.TicksPerMillisecond * 5;
    
    public TelegramClientCheckerTask(ILogger<TelegramClientCheckerTask> log, TelegramClientService botService) : base(log)
    {
        _log = log;
        _tgClient = botService;
    }

    protected override async Task Run()
    {
        var clientStart = _tgClient.ClientCreatedAt;
        if (DateTime.UtcNow - clientStart < TimeSpan.FromMinutes(3))
        {
            return;
        }
        long lastUpdateTicks = _tgClient.GetLastPong();
        long tickFromLastUpdate = Environment.TickCount64 - lastUpdateTicks;
        if (tickFromLastUpdate < TimeoutTicks)
        {
            return;
        }
        _log.Warning("Ping-Pong update timeout exceeded, now i'll proceed to recreate the client");
        while (true)
        {
            try
            {
                await _tgClient.DisconnectAndClearAsync();
                await _tgClient.Connect(true);
                break;
            }
            catch (SocketException se)
            {
                _log.Error(se, "Unable to connect, socket exception");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable to connect telegram client");
            }
            await Task.Delay(5000);
        }
    }
}
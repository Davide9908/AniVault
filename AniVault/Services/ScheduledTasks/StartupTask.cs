
using AniVault.Database.Context;
using AniVault.Services.Extensions;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using System.Linq;

namespace AniVault.Services.ScheduledTasks;

public class StartupTask : BaseTask
{
    private readonly ILogger<StartupTask> _log;
    private readonly TelegramClientService _clientService;
    private readonly IServiceProvider _serviceProvider;
    private readonly AniVaultDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    public StartupTask(ILogger<StartupTask> log, TelegramClientService clientService, IServiceProvider serviceProvider, IConfiguration configuration, IHostEnvironment hostEnvironment, AniVaultDbContext dbContext) : base(log)
    {
        _log = log;
        _clientService = clientService;
        _serviceProvider = serviceProvider;
        _dbContext =  dbContext;
        _configuration = configuration;
        _isDevelopment = hostEnvironment.IsDevelopment();
    }

    protected override async Task Run()
    {
        _log.Info("Starting Telegram Client");
        await _clientService.Connect(true);

        var tasks = _dbContext.ScheduledTasks.ToList();
        _serviceProvider.UseScheduler(scheduler =>
            {
                foreach (var task in tasks)
                {
                    if (!task.IntervalSeconds.HasValue)
                    {
                        continue;
                    }
                    
                    var scheduledTask = scheduler.ScheduleInvocableType(GetTypeByName(task.TaskName));
                    CalculateInterval(task.IntervalSeconds.Value, scheduledTask)
                        .PreventOverlapping(task.TaskName);
                }
                scheduler.Schedule<UpdateManagerSaveStateTask>()
                    .EveryThirtySeconds()
                    .PreventOverlapping(nameof(UpdateManagerSaveStateTask));
                //scheduler.ScheduleInvocableType()
                // scheduler.Schedule<PowerAlertTask>()
                //     .EverySeconds(Constants.Every3Seconds)
                //     .PreventOverlapping(nameof(PowerAlertTask));
                // scheduler.Schedule<GithubReleasesCheckerTask>()
                //     .DailyAtHour(14)
                //     .Zoned(TimeZoneInfo.Local)
                //     .PreventOverlapping(nameof(GithubReleasesCheckerTask));
                // scheduler.Schedule<GithubReleaseDownloadTask>()
                //     .EveryThirtyMinutes()
                //     .PreventOverlapping(nameof(GithubReleaseDownloadTask))
                //     .RunOnceAtStart();
                // scheduler.Schedule<SendReleaseAssetTask>()
                //     .Cron(Constants.Every25MinutesCron)
                //     .PreventOverlapping(nameof(SendReleaseAssetTask));
            })
            .LogScheduledTaskProgress();
    }

    private static Type GetTypeByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().AsEnumerable()
            .Reverse()
            .Select(assembly => assembly.GetType(name))
            .FirstOrDefault(t => t is not null)!;
    }

    private IScheduledEventConfiguration CalculateInterval(int intervalSeconds, IScheduleInterval? scheduleInterval)
    {
        if (intervalSeconds < 60)
        {
            return scheduleInterval?.EverySeconds(intervalSeconds)!;
        }
        if (intervalSeconds < 3600)
        {
            int minutes = intervalSeconds / 60;
            return scheduleInterval?.Cron($"*/{minutes} * * * *")!;
        }
        int hours = intervalSeconds / 3600;
        return scheduleInterval?.Cron($"{DateTime.Now.Minute} */{hours} * * *")!;
    }

}
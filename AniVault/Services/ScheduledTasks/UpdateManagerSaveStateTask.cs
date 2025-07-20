using AniVault.Database.Context;

namespace AniVault.Services.ScheduledTasks;

public class UpdateManagerSaveStateTask : BaseTask
{
    private readonly TelegramClientService _telegramClientService;
    public UpdateManagerSaveStateTask(ILogger<UpdateManagerSaveStateTask> log, TelegramClientService telegramClient) : base(log)
    {
        _telegramClientService = telegramClient;
    }

    protected override Task Run()
    {
        _telegramClientService.SaveStateUpdateManager();
        return Task.CompletedTask;
    }
}
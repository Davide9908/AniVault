using AniVault.Database;
using AniVault.Database.Context;
using AniVault.Services.Extensions;
using Coravel.Invocable;

namespace AniVault.Services.ScheduledTasks;

public abstract class BaseTask : IInvocable, ICancellableInvocable
{
    private readonly ILogger _log;
    private readonly AniVaultDbContext? _dbContext;
    private readonly string _taskName;

    //if there is no dbContext it means that the child task doesn't require updating start/end dates in the database
    protected BaseTask(ILogger log, AniVaultDbContext? dbContext = null)
    {
        _log = log;
        _dbContext = dbContext;
        _taskName = GetType().FullName;
    }

    public virtual async Task Invoke()
    {
        
        ScheduledTask? task = null;
        if (_dbContext is not null)
        {
            task = GetUpdateTaskStart();
        }

        if (task is not null && !task.Enabled)
        {
            return;
        }
        
        try
        {
            await Run();
        }
        catch (OperationCanceledException oce)
        {
            _log.Warning(oce, "Task cancelled");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Errore nell'esecuzione del task {taskName}", GetType().Name);
        }
        finally
        {
            if (task is not null)
            {
                UpdateTaskEnd();
            }
        }

    }

    protected ScheduledTask GetUpdateTaskStart(AniVaultDbContext? customDbContext = null)
    {
        if (customDbContext is not null)
        {
            var taskCustomDb = customDbContext!.ScheduledTasks.First(t => t.TaskName == _taskName);
            if (!taskCustomDb.Enabled)
            {
                return taskCustomDb;
            }
            taskCustomDb.LastStart = DateTime.UtcNow;
            customDbContext.SaveChanges();
            return taskCustomDb;
        }
        
        var task = _dbContext!.ScheduledTasks.First(t => t.TaskName == _taskName);
        if (!task.Enabled)
        {
            return task;
        }
        task.LastStart = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return task;
    }

    protected void UpdateTaskEnd(AniVaultDbContext? customDbContext = null)
    {
        if (customDbContext is not null)
        {
            var taskCustomDb = _dbContext!.ScheduledTasks.First(t => t.TaskName == _taskName);
            taskCustomDb.LastFinish = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return;
        }
        
        var task = _dbContext!.ScheduledTasks.First(t => t.TaskName == _taskName);
        task.LastFinish = DateTime.UtcNow;
        _dbContext.SaveChanges();
    }
    

    protected abstract Task Run();

    protected CancellationToken _ct;

    public CancellationToken CancellationToken { get => _ct; set  => _ct = value; }
}
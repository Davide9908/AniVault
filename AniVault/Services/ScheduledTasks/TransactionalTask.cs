using AniVault.Database.Context;
using AniVault.Services.Extensions;

namespace AniVault.Services.ScheduledTasks;

public abstract class TransactionalTask : BaseTask
{
    private readonly ILogger _log;
    private readonly AniVaultDbContext? _dbContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    protected TransactionalTask(ILogger log, AniVaultDbContext context, IServiceScopeFactory scopeFactory): base(log)
    {
        _log = log;
        _dbContext = context;
        _serviceScopeFactory = scopeFactory;
    }

    public sealed override async Task Invoke()
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();
        await using var noTransactionDbContext = serviceScope.ServiceProvider.GetRequiredService<AniVaultDbContext>();
        
        var task = GetUpdateTaskStart(noTransactionDbContext);
        
        if ( !task.Enabled)
        {
            return;
        }
        if (_dbContext is null)
        {
            await Run();
            UpdateTaskEnd(noTransactionDbContext);
            return;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(_ct);
        try
        {
            await Run();
            await transaction.CommitAsync(CancellationToken.None);
        }
        catch (OperationCanceledException oce)
        {
            _log.Warning(oce, "Task cancelled");
            await transaction.RollbackAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Errore nell'esecuzione del task {taskName}", GetType().Name);
            await transaction.RollbackAsync(CancellationToken.None);
        }
        finally
        {
            UpdateTaskEnd(noTransactionDbContext);
        }

    }

    protected abstract override Task Run();

    private static AniVaultDbContext CreateOutTransactionDbContext(IServiceScopeFactory scopeFactory)
    {
        var newScope = scopeFactory.CreateScope();
        return newScope.ServiceProvider.GetRequiredService<AniVaultDbContext>();
    }

}
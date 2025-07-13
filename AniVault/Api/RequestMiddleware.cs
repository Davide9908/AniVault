using AniVault.Database.Context;

namespace AniVault.Api;

public class RequestMiddleware
{
    private readonly RequestDelegate _next;

    public RequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AniVaultDbContext dbContext)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await _next(context);

            // Only commit if there was no error
            if (context.Response.StatusCode < 400)
            {
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
}
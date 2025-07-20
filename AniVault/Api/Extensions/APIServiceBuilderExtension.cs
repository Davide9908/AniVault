using AniVault.Database;
using AniVault.Database.Context;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

namespace AniVault.Api.Extensions;

public static class ApiServiceBuilderExtension
{
    public static IServiceCollection SetupApiServices(this IServiceCollection services)
    {
        services.AddAuthentication();
        // Scalar OpenAPI Setup
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>();
        });
        return services.AddEndpointsApiExplorer();
    }

    public static void SetupApiConfiguration(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        //  Middleware: controlla attributo RequireApiKeyAttribute
        app.Use(async (context, next) =>
        {
    
            var endpoint = context.GetEndpoint();
            var requiresApiKey = endpoint?.Metadata?.GetMetadata<RequireApiKeyAttribute>() != null;
    
            if (requiresApiKey)
            {
                
                if (!context.Request.Headers.TryGetValue("x-api-key", out var keySv))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
                string key = keySv.ToString();
                var dbContext = context.RequestServices.GetRequiredService<AniVaultDbContext>();
                var user = dbContext.ApiUsers.FirstOrDefault(u => u.ApiKey == key);
                if (user is null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                context.Items.Add("userId", user.ApiUserId);
                // var userId = 1;
                // context.Items.Add("userId", userId);
            }
    
            await next();
        });
        app.UseMiddleware<RequestMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            var securityApi = app.MapGroup("/security");
            securityApi.MapPost("/addApiUser", (string username, AniVaultDbContext dbContext) =>
            {
                var apiUser = new ApiUser(username);
                dbContext.ApiUsers.Add(apiUser);
                dbContext.SaveChanges();
                return apiUser.ApiKey;
            });
        }

        app.MapTelegramClientApis();

        // var todosApi = app.MapGroup("/todos");
        // todosApi.MapGet("/hello", () => "Hello world!");
        // todosApi.MapGet("/helloSecure", (HttpContext context) =>
        //     {
        //         var userId = (int)context.Items["userId"];
        //         return $"hello user {userId}";
        //     })
        //     .UseSecurity();
    }
}

using AniVault.Database.Context;
using DBType = ASql.ASqlManager.DBType;
using Coravel;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Database;

namespace AniVault.Services.Extensions;

public static class ServiceBuilderExtension
{
    public static IServiceCollection SetupServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        services.AddDbContext<AniVaultDbContext>()
            .AddSingleton<TelegramClientService>()
            .AddScoped<TelegramClientApiService>()
            .AddScheduler()
            .AddSerilog(serilogConfig =>
            {
                if (env.IsDevelopment())
                {
                    serilogConfig = serilogConfig.MinimumLevel.Debug().WriteTo.Console();
                }
                else
                {
                    serilogConfig = serilogConfig.MinimumLevel.Information().WriteTo.Console();
                }

                string? connectionString = configuration.GetConnectionString("AnimeVaultDB");
                if (connectionString is null)
                {
                    throw new ApplicationException("Could not find connection string");
                }

                serilogConfig.WriteTo.Database(DBType.PostgreSQL, connectionString, "system_log",
                    LogEventLevel.Debug, false, 1);
            });
        // .AddHttpClient<GithubApiHttpClientService>(options =>
        // {
        //     options.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.HeaderUserAgent);
        //     options.DefaultRequestHeaders.Add("Authorization", configuration["GitHubAccessToken"]);
        // });
        return services;
    }
}
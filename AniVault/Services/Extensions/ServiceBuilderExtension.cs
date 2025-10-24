
using AniVault.Database.Context;
using AniVault.Services.Classes;
using AniVault.Services.ScheduledTasks;
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
            .AddScoped<UpdateManagerSaveStateTask>()
            .AddScoped<StartupTask>()
            .AddScoped<DownloadEpisodesTask>()
            .AddScoped<CompletedEpisodesMoverTask>()
            .AddScoped<AnimeEpisodeService>()
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
            })
            .AddHttpClient<MalApiHttpClientService>(options =>
            {
                string baseUrl = configuration.GetRequiredSection("MalApiSettings").GetValue<string>("MALApiLink") ?? throw new InvalidOperationException("MalApiSettings : MALApiLink not set");
                string headerApiId = configuration.GetRequiredSection("MalApiSettings").GetValue<string>("MALApiID") ?? throw new InvalidOperationException("MalApiSettings : MALApiID not set");
                options.BaseAddress = new Uri(baseUrl);
                options.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", headerApiId);
            });
        return services;
    }
}
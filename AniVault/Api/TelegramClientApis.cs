using AniVault.Api.Extensions;
using AniVault.Database;
using AniVault.Database.Context;
using AniVault.Services;
using AniVault.Services.Classes;
using Microsoft.AspNetCore.Mvc;

namespace AniVault.Api;

public static class TelegramClientApis
{
    public static void MapTelegramClientApis(this WebApplication app)
    {
        var tgClientApi = app.MapGroup("/tgClient").WithTags("telegram Apis");
        tgClientApi.MapPost("/loadChannelsMessages", async (HttpContext context, CancellationToken ct, TelegramClientApiService service, string? mainChannelName) =>
            {
                await service.LoadMissingChannelsAndMessages(context.GetUserId(), mainChannelName, ct);
            })
            .UseSecurity();
        tgClientApi.MapPost("/forceLoadMessageFromIdByDbChannelId",
            async (HttpContext context, CancellationToken ct, TelegramClientApiService service, LoadMessageFromIdByDbChannelId request) =>
            {
                await service.ForceLoadMessageFromIdByDbChannelId(context.GetUserId(), request.DbChannelId, request.MessageId, ct);
            })
            .UseSecurity();
        tgClientApi.MapGet("/getFileDownloadInError",
            (HttpContext context, AniVaultDbContext dbContext) =>
            {
                return dbContext.TelegramMediaDocuments.Where(x => x.DownloadStatus == DownloadStatus.Error)
                    .Select(x => x.Filename).ToList();
            });
        tgClientApi.MapPost("/retryOneDownload",
            (HttpContext context, AniVaultDbContext dbContext) =>
            {
                var file = dbContext.TelegramMediaDocuments.First(x => x.DownloadStatus == DownloadStatus.Error);
                file.DownloadStatus = DownloadStatus.NotStarted;
                dbContext.SaveChanges();
            });
    }
}
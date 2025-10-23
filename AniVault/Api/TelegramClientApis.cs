using AniVault.Api.Extensions;
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
    }
}
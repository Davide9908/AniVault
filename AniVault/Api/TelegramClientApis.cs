using AniVault.Api.Extensions;
using AniVault.Services;

namespace AniVault.Api;

public static class TelegramClientApis
{
    public static void MapTelegramClientApis(this WebApplication app)
    {
        var todosApi = app.MapGroup("/tgClient").WithTags("telegram Apis");
        todosApi.MapPost("/loadChannelsMessages", async (HttpContext context, CancellationToken ct, TelegramClientApiService service) =>
            {
                await service.LoadMissingChannelsAndMessages(context.GetUserId(), ct);
            }).UseSecurity();
    }
}
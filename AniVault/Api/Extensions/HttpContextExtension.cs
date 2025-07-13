namespace AniVault.Api.Extensions;

public static class HttpContextExtension
{
    public static int GetUserId(this HttpContext context)
    {
        return (int)context.Items["userId"]!;
    }
}
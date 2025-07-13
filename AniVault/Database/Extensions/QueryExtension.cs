namespace AniVault.Database.Extensions;

public static class QueryExtension
{
    public static IQueryable<ApiUser> GetById(this IQueryable<ApiUser> apiUsers, int apiUserId)
    {
        return apiUsers.Where(a => a.ApiUserId == apiUserId);
    }
}
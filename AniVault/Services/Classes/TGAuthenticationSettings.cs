namespace AniVault.Services.Classes;

public record TGAuthenticationSettings(
    string ApiId,
    string ApiHash,
    string SessionPath,
    string PhoneNumber,
    string Password);

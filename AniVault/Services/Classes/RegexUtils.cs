using System.Text.RegularExpressions;

namespace AniVault.Services.Classes;

public static partial class RegexUtils
{
    [GeneratedRegex(@"#ep[0-9]{1,3}", RegexOptions.IgnoreCase, "it-IT")]
    public static partial Regex EpRegex();

    [GeneratedRegex(@"[^.]+$", RegexOptions.IgnoreCase | RegexOptions.RightToLeft, "it-IT")]
    public static partial Regex FileExtensionRegex();
}
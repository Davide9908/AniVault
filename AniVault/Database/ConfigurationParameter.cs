using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    
    public class SystemConfigurationParameter
    {
        public SystemConfigurationParameter() { }

        [Key]
        public int ConfigurationParameterId { get; set; }

        [Required, MaxLength(50)]
        public string ParameterName { get; set; }

        public string? ParameterValue { get; set; }

        public ConfigurationParameterType ParameterType { get; set; }
    }

    public enum ConfigurationParameterType
    {
        SysReserved = 0,
        Int,
        String,
        Bool,
        DateTime,
        Decimal
    }

    public static class ParameterNames
    {
        public const string DefaultDownloadLocation = nameof(DefaultDownloadLocation);
        public const string TGApiId = nameof(TGApiId);
        public const string TGApiHash = nameof(TGApiHash);
        public const string SessionPath = nameof(SessionPath);
        public const string PhoneNumber = nameof(PhoneNumber);
        public const string Password = nameof(Password);
        public const string WTelegramClientLogPath = nameof(WTelegramClientLogPath);
        public const string LastChatsRefresh = nameof(LastChatsRefresh);
        public const string MALApiLink = nameof(MALApiLink);
        public const string MALApiID = nameof(MALApiID);
        public const string MALUsername = nameof(MALUsername);
        public const string RefreshDBChannels = nameof(RefreshDBChannels);
    }
}

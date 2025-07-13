using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    [Table(nameof(TelegramChannel))]
    public class TelegramChannel
    {
        [Key]
        public int TelegramChannelId { get; set; }
        
        public long ChatId { get; set; }

        public long AccessHash { get; set; }
        
        public string ChannelName { get; set; }

        public bool AutoDownloadEnabled { get; set; }

        public ChannelStatus Status { get; set; }
        
        public AnimeEpisodesSetting? AnimeEpisodesSetting { get; set; }

        public List<TelegramMessage> TelegramMessages { get; set; } = [];

        public TelegramChannel()
        {

        }
        public TelegramChannel(long id, long accessHash, string channelName)
        {
            ChatId = id;
            AccessHash = accessHash;
            ChannelName = channelName;
            AutoDownloadEnabled = false;
            Status = ChannelStatus.Active;
        }

        public override string ToString()
        {
            return string.Join(" - ", ChatId, AccessHash, ChannelName);
        }
    }

    public enum ChannelStatus
    {
        SysReserved = 0,
        Active,
        Deleted
    }
}

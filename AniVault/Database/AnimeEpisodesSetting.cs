using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    [Table(nameof(AnimeEpisodesSetting))]
    public class AnimeEpisodesSetting
    {
        [ForeignKey(nameof(TelegramChannel))]
        public int TelegramChannelId { get; set; }
        public TelegramChannel TelegramChannel { get; set; }

        public string? FileNameTemplate { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MALAnimeId { get; set; }

        public string? AnimeFolderPath { get; set; }

        public bool DownloadLastEpisode { get; set; }

        public short? CourEpisodeNumberGap { get; set; }

        public bool UseGapForEpNum { get; set; }
    }
}

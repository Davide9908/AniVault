using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    [Table(nameof(AnimeConfiguration))]
    public class AnimeConfiguration
    {
        public int AnimeConfigurationId { get; set; }
        
        [MaxLength(100)]
        public string AnimeName { get; set; }
        
        public int? MyAnimeListId { get; set; }
        
        [MaxLength(150)]
        public string? FileNameTemplate { get; set; }

        /// <summary>Anime folder path related to configuration's LibraryPath</summary>
        [MaxLength(250)]
        public string? AnimeFolderRelativePath { get; set; }
        
        public bool AutoDownloadEnabled { get; set; }

        public short? EpisodesNumberOffset { get; set; }
        
        public DateTime CreationDateTime { get; set; }
        
        public List<TelegramMediaDocument> RelatedEpisodes { get; set; } = [];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animeName"></param>
        public AnimeConfiguration(string animeName)
        {
            AnimeName = animeName;
            AutoDownloadEnabled = false;
            CreationDateTime = DateTime.UtcNow;
        }
    }
}

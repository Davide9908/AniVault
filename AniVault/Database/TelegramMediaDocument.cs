using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    [Table("telegram_media_document")]
    public class TelegramMediaDocument
    {
        [Key]
        public int TelegramMediaDocumentId { get; set; }
        
        public long FileId { get; set; }

        public long AccessHash { get; set; }
        
        public byte[] FileReference { get; set; }
        
        [ForeignKey(nameof(TelegramMessage))]
        public int TelegramMessageId { get; set; }
        public TelegramMessage TelegramMessage { get; set; }
        
        [ForeignKey(nameof(AnimeConfiguration))]
        public int AnimeConfigurationId { get; set; }
        public AnimeConfiguration AnimeConfiguration { get; set; }

        [MaxLength(80)]
        public string Filename { get; set; }
        
        [MaxLength(100)]
        public string FilenameFromTelegram  {get; set;}
        
        [MaxLength(80)]
        public string? FilenameToUpdate { get; set; }
        
        [MaxLength(30)]
        public string MimeType { get; set; } 

        public long Size { get; set; }

        public long DataTransmitted { get; set; }

        public DateTime CreationDateTime { get; set; }
        
        public DateTime LastUpdateDateTime { get; set; }

        public DownloadStatus DownloadStatus { get; set; }


        public TelegramMediaDocument()
        {
            DataTransmitted = 0;
            CreationDateTime = DateTime.UtcNow;
            LastUpdateDateTime = DateTime.UtcNow;
            DownloadStatus = DownloadStatus.NotStarted;
        }

        public TelegramMediaDocument(long fileId, long accessHash, byte[] fileReference, string filename, string filenameFromTelegram, long size, string mimeType, AnimeConfiguration animeConfiguration) : this()
        {
            FileId = fileId;
            AccessHash = accessHash;
            FileReference = fileReference;
            Filename = filename;
            FilenameFromTelegram = filenameFromTelegram;
            Size = size;
            MimeType = mimeType;
            AnimeConfiguration = animeConfiguration;
        }
        
        public TelegramMediaDocument(long fileId, long accessHash, byte[] fileReference, TelegramMessage telegramMessage, string filename, string filenameFromTelegram, long size, string mimeType, AnimeConfiguration animeConfiguration) : this()
        {
            FileId = fileId;
            AccessHash = accessHash;
            FileReference = fileReference;
            TelegramMessage = telegramMessage;
            Filename = filename;
            FilenameFromTelegram = filenameFromTelegram;
            Size = size;
            MimeType = mimeType;
            AnimeConfiguration = animeConfiguration;
        }
    }
    
    public enum DownloadStatus
    {
        SysReserved = 0,
        NotStarted,
        Downloading,
        Error,
        Completed,
        Aborted,
        Ignored
    }


}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    [Table(nameof(TelegramMediaDocument))]
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

        [MaxLength(80)]
        public string Filename { get; set; }
        
        [MaxLength(100)]
        public string FilenameFromTelegram  {get; set;}
        
        [MaxLength(80)]
        public string? FilenameToUpdate { get; set; }
        
        [MaxLength(20)]
        public string MimeType { get; set; } 

        public long Size { get; set; }

        public long DataTransmitted { get; set; }

        public DateTime LastUpdate { get; set; }

        public DownloadStatus DownloadStatus { get; set; }

        public DownloadErrorType? ErrorType { get; set; }

        public TelegramMediaDocument()
        {
            DataTransmitted = 0;
            LastUpdate = DateTime.UtcNow;
            ErrorType = null;
            DownloadStatus = DownloadStatus.NotStarted;
        }

        public TelegramMediaDocument(long fileId, long accessHash, byte[] fileReference, string filename, string filenameFromTelegram, long size, string mimeType) : this()
        {
            FileId = fileId;
            AccessHash = accessHash;
            FileReference = fileReference;
            Filename = filename;
            FilenameFromTelegram = filenameFromTelegram;
            Size = size;
            MimeType = mimeType;
        }
        
        public TelegramMediaDocument(long fileId, long accessHash, byte[] fileReference, TelegramMessage telegramMessage, string filename, string filenameFromTelegram, long size, string mimeType) : this()
        {
            FileId = fileId;
            AccessHash = accessHash;
            FileReference = fileReference;
            TelegramMessage = telegramMessage;
            Filename = filename;
            FilenameFromTelegram = filenameFromTelegram;
            Size = size;
            MimeType = mimeType;
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
    public enum DownloadErrorType
    {
        SysReserved = 0,
        NetworkIssue,
        Cancelled,
        Other
    }


}

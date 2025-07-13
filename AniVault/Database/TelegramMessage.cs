using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniVault.Database
{
    [Table(nameof(TelegramMessage))]
    public class TelegramMessage
    {
        /// <summary>
        /// The id of the table. Do not use with tg client
        /// </summary>
        public int TelegramMessageId { get; set; }

        /// <summary>
        /// The actual tg id
        /// </summary>
        public int MessageId { get; set; }

        [MaxLength(1000)] 
        public string MessageText { get; set; } = null!;

        public DateTime ReceivedDatetime { get; set; }

        public DateTime? UpdateDatetime { get; set; }
        
        public MessageStatus MessageStatus { get; set; }

        [ForeignKey(nameof(TelegramChannel))] 
        public int TelegramChannelId { get; set; }
        public TelegramChannel TelegramChannel { get; set; } = null!;

        public TelegramMediaDocument? MediaDocument { get; set; }
        
        public TelegramMessage()
        {
            ReceivedDatetime = DateTime.Now;
            MessageStatus = MessageStatus.Active;
        }

        public void Delete()
        {
            MessageStatus = MessageStatus.Deleted;
        }
    }

    public enum MessageStatus
    {
        Active = 1,
        Deleted
    }
}
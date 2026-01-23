using EventAssist.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventAssist.Models.Records
{
    [Table("Messages")]
    public class MessageRecord
    {
        public int Id { get; set; }
        public MessageType Type { get; set; }
        public MessageSender Sender { get; set; }
        public required string Text { get; set; }
        public string? FunctionCall { get; set; }
        public string? FunctionResponse { get; set; }
        public DateTime SentDate { get; set; }
        public int ChatId { get; set; }
        public ChatRecord? Chat { get; set; }
    }
}

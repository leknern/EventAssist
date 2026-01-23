using EventAssist.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventAssist.Models.Records
{
    [Table("Chats")]
    public class ChatRecord
    {
        public int Id { get; set; }
        public ChatStatus Status { get; set; }
        public bool HumanSupportRequired { get; set; }
        public int UserId { get; set; }
        public UserRecord? User { get; set; }
        public int? CustomerSupportAgentId { get; set; }
        public UserRecord? CustomerSupportAgent { get; set; }
        public List<MessageRecord>? Messages { get; set; }
        public string? InternalNote { get; set; }
        public bool IsRead { get; set; }
        public DateTime Created { get; set; }
    }
}

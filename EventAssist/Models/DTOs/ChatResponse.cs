using EventAssist.Models.Enums;

namespace EventAssist.Models.DTOs
{
    public class ChatResponse
    {
        public int Id { get; set; }
        public ChatStatus Status { get; set; }
        public required ChatMember User { get; set; }
        public ChatMember? CustomerSupportAgent { get; set; }
        public bool HumanSupportRequired { get; set; }
        public string? InternalNote { get; set; }
        public MessageResponse? LastMessage { get; set; }
        public DateTime Created { get; set; }
    }
}

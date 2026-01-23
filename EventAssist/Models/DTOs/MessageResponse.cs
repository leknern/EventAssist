using EventAssist.Models.Enums;

namespace EventAssist.Models.DTOs
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public MessageType Type { get; set; }
        public MessageSender Sender { get; set; }
        public required string Text { get; set; }
        public string? FunctionCall { get; set; }
        public string? FunctionResponse { get; set; }
        public DateTime SentDate { get; set; }
    }
}

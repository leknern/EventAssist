namespace EventAssist.Models.DTOs
{
    public class CustomerSupportCommentRequest
    {
        public int ChatId { get; set; }
        public required string Comment { get; set; }
    }
}

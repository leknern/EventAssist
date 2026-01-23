namespace EventAssist.Models.DTOs
{
    public class ChatMember
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ProfilePictureUrl { get; set; }
        public bool IsOnline { get; set; }
    }
}

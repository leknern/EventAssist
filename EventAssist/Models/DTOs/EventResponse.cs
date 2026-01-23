namespace EventAssist.Models.DTOs
{
    public class EventResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Occurrence { get; set; }
    }
}

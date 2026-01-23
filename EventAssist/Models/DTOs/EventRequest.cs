using System.ComponentModel.DataAnnotations;

namespace EventAssist.Models.DTOs
{
    public class EventRequest
    {
        [MaxLength(200)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime Occurrence { get; set; }
    }
}

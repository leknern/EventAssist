using System.ComponentModel.DataAnnotations;

namespace EventAssist.Models.DTOs
{
    public class EventDescriptionRequest
    {
        public int Id { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}

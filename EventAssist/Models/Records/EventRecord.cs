using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventAssist.Models.Records
{
    [Table("Events")]
    public class EventRecord
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime Occurrence { get; set; }
        public int UserRecordId { get; set; }
        public required UserRecord UserRecord { get; set; }
    }
}

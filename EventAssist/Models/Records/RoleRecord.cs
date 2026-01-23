using System.ComponentModel.DataAnnotations.Schema;

namespace EventAssist.Models.Records
{
    [Table("Roles")]
    public class RoleRecord
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required List<UserRecord> Users { get; set; }
    }
}

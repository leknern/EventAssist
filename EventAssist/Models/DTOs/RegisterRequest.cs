using System.ComponentModel.DataAnnotations;

namespace EventAssist.Models.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [Length(minimumLength: 2, maximumLength: 50)]
        public required string Name { get; set; }
        [Required]
        [RegularExpression(pattern: @"^\S+@\S+\.\S+$")]
        public required string Email { get; set; }
        [Required]
        [Length(minimumLength: 8, maximumLength: 32)]
        [RegularExpression(pattern: @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$")]
        public required string Password { get; set; }
        [Required]
        public required string ProfilePictureUrl { get; set; }
    }
}

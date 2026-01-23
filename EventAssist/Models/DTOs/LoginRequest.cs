using System.ComponentModel.DataAnnotations;

namespace EventAssist.Models.DTOs
{
    public class LoginRequest
    {
        [Required]
        [RegularExpression(pattern: @"^\S+@\S+\.\S+$")]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}

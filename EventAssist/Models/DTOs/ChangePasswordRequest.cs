using System.ComponentModel.DataAnnotations;

namespace EventAssist.Models.DTOs
{
    public class ChangePasswordRequest
    {
        public required string Password { get; set; }

        [Length(minimumLength: 8, maximumLength: 32)]
        [RegularExpression(pattern: @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$")]
        public required string NewPassword { get; set; }

    }
}

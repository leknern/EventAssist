using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventAssist.Models.Records
{
    [Table("Users")]
    public class UserRecord
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string ProfilePictureUrl { get; set; }
        public required byte[] PasswordHash { get; set; }
        public required byte[] PasswordSalt { get; set; }
        public bool IsTwoFactorAuthEnabled { get; set; }
        public string? TwoFactorAuthSecret { get; set; }
        public string? PasswordResetToken { get; set; }
        public int LoginErrorCount { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastLoginErrorDate { get; set; }
        public DateTime LockoutEndDate { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public required List<RoleRecord> Roles { get; set; }
        public required List<EventRecord> Events { get; set; }
        public required List<ChatRecord> Chats { get; set; }
        /// <summary>
        /// Gets or sets the concurrency token for the user entity.
        /// </summary>
        /// 
        /// <remarks>
        /// This property is used by Entity Framework for optimistic concurrency control.
        /// The [Timestamp] attribute ensures that if multiple processes attempt to
        /// update the same user record simultaneously, changes will not overwrite
        /// each other unintentionally. EF will check the Version value when saving
        /// and throw a DbUpdateConcurrencyException if the record has been modified
        /// by another process since it was loaded.
        /// </remarks>
        [Timestamp]
        public byte[]? Version { get; set; }
    }
}

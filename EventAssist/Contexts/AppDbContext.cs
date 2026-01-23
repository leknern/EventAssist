using EventAssist.Models.Records;
using Microsoft.EntityFrameworkCore;

namespace EventAssist.Contexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ChatRecord> Chats { get; set; }
        public DbSet<EventRecord> Events { get; set; }
        public DbSet<MessageRecord> Messages { get; set; }
        public DbSet<UserRecord> Users { get; set; }
        public DbSet<RoleRecord> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatRecord>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRecord>()
                .HasOne(c => c.CustomerSupportAgent)
                .WithMany()
                .HasForeignKey(c => c.CustomerSupportAgentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

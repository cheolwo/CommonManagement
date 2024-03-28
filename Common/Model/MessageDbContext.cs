using Microsoft.EntityFrameworkCore;
using System;

namespace Common.Model
{
    public class MessageDbContext : DbContext
    {
        public DbSet<ErrorMessageEntry> ErrorMessages { get; set; }

        public MessageDbContext(DbContextOptions<MessageDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ErrorMessageEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.JsonData).IsRequired();
                entity.Property(e => e.TypeName).IsRequired();
                entity.Property(e => e.ErrorMessage).IsRequired();
                entity.Property(e => e.TimeStamp).IsRequired();
            });
        }
    }

    public class ErrorMessageEntry
    {
        public int Id { get; set; }
        public string JsonData { get; set; }
        public string TypeName { get; set; }
        public string ErrorMessage { get; set; }
        public string TimeStamp { get; set; }
    }
}

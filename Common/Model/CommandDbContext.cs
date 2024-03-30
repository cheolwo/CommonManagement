using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Common.Model
{
    public class CommandDbContext : DbContext
    {
        public DbSet<CommandLog> CommandLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommandLog>()
                .Property(e => e.CommandData)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string>(v));

            base.OnModelCreating(modelBuilder);
        }

        // 공통 DbContext 설정을 여기에 정의
    }

}

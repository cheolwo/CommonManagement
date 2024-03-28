using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Common.Services
{
    public class Message
    {
        public int Id { get; set; }
        public string JsonData { get; set; }
        public string TypeName { get; set; }
        public DateTime TimeStamp { get; set; } // 시간 순서 기록을 위한 TimeStamp 필드 추가
    }

    public class MessageDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().HasKey(m => m.Id);

            // TimeStamp 필드에 기본값으로 현재 시간을 설정
            modelBuilder.Entity<Message>()
                .Property(m => m.TimeStamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
    public class MessageBrokerService
    {
        private readonly IMediator _mediator;
        private readonly MessageDbContext _dbContext;

        public MessageBrokerService(IMediator mediator, MessageDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task SaveDtoAsync<TDto>(TDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var typeName = dto.GetType().AssemblyQualifiedName;

            var message = new Message { JsonData = json, TypeName = typeName };
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ProcessStoredDtosAsync()
        {
            // TimeStamp 기준으로 오름차순 정렬하여 메시지 로드
            var messages = await _dbContext.Messages.OrderBy(m => m.TimeStamp).ToListAsync();

            foreach (var message in messages)
            {
                var type = Type.GetType(message.TypeName);
                var dto = JsonConvert.DeserializeObject(message.JsonData, type);

                if (dto is IRequest request)
                {
                    await _mediator.Send(request);
                }

                _dbContext.Messages.Remove(message);
            }

            await _dbContext.SaveChangesAsync();
        }

    }
}

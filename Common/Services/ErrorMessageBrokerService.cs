using Common.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Common.Services
{
    public interface IErrorMessageBrokerService
    {
        Task SaveFailedCommandAsync<TCommand>(TCommand Command, string errorMessage);
        Task RetryFailedCommandsAsync();
    }
    public class ErrorMessageBrokerService : IErrorMessageBrokerService
    {
        private readonly IMediator _mediator;
        private readonly MessageDbContext _dbContext;

        public ErrorMessageBrokerService(IMediator mediator, MessageDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        // 실패한 Command 저장
        public async Task SaveFailedCommandAsync<TCommand>(TCommand Command, string errorMessage)
        {
            var json = JsonConvert.SerializeObject(Command);
            var typeName = Command.GetType().AssemblyQualifiedName;
            var timeStamp = DateTime.UtcNow;

            var errorMessageEntry = new Message
            {
                JsonData = json,
                TypeName = typeName,
                TimeStamp = timeStamp
            };

            _dbContext.Messages.Add(errorMessageEntry);
            await _dbContext.SaveChangesAsync();
        }
        // 저장된 실패한 Command 처리
        public async Task RetryFailedCommandsAsync()
        {
            var errorMessages = await _dbContext.Messages.ToListAsync();

            foreach (var errorMessage in errorMessages)
            {
                var type = Type.GetType(errorMessage.TypeName);
                if (type == null) continue; // Type이 유효하지 않은 경우 건너뛰기

                var Command = JsonConvert.DeserializeObject(errorMessage.JsonData, type);

                if (Command is IRequest request)
                {
                    await _mediator.Send(request);
                    // 성공적으로 처리된 경우, 에러 메시지 엔트리 삭제
                    _dbContext.Messages.Remove(errorMessage);
                }
                else
                {
                    // 처리할 수 없는 타입의 Command인 경우 로깅 or 추가 조치
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}

using Common.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Command
{
    public class CommandHandlr<TCommand> : IRequestHandler<TCommand, bool> where TCommand : IRequest<bool>
    {
        private readonly ICommandService<TCommand> _commandService;
        private readonly ITokenStorageService _tokenStorageService;
        private readonly ILogger<CommandHandlr<TCommand>> _logger;
        private readonly IErrorMessageBrokerService _errorMessageBrokerService;

        public CommandHandlr(ICommandService<TCommand> commandService, 
                             ITokenStorageService tokenStorageService, 
                             ILogger<CommandHandlr<TCommand>> logger, 
                             IErrorMessageBrokerService errorMessageBrokerService)
        {
            _commandService = commandService;
            _tokenStorageService = tokenStorageService;
            _logger = logger;
            _errorMessageBrokerService = errorMessageBrokerService;
        }

        public virtual async Task<bool> Handle(TCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _tokenStorageService.LoadTokenAsync();
                await _commandService.CreateAsync(token, request, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 취소 로직 처리
                return false;
            }
            catch (Exception ex)
            {
                // 오류 메시지를 저장하고 로그에 기록
                await _errorMessageBrokerService.SaveFailedCommandAsync(request, ex.Message);
                _logger.LogError(ex, "Order creation failed for request: {@Request}", request);

                // 예외를 다시 던지지 않고, 실패했다는 것을 나타내는 false 반환
                return false;
            }

            // 성공 로그 남기기
            _logger.LogInformation("Order created successfully for request: {@Request}", request);
            return true;
        }
    }
}

using Common.Model;
using FrontCommon.Actor;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Command
{
    /// <summary>
    /// 클라이언트 측에서 CUD 작업을 담당하는 서비스 인터페이스입니다.
    /// </summary>
    /// <typeparam name="T">작업 대상 엔티티 타입</typeparam>
    public interface ICommandService<T> where T : IRequest<bool>
    {
        /// <summary>
        /// 엔티티를 생성하고 서버에 전송합니다.
        /// </summary>
        /// <param name="jwtToken">JWT 인증 토큰</param>
        /// <param name="entity">생성할 엔티티</param>
        Task CreateAsync(string jwtToken, T command, CancellationToken cancellationToken);


        /// <summary>
        /// 엔티티를 갱신하고 서버에 전송합니다.
        /// </summary>
        /// <param name="jwtToken">JWT 인증 토큰</param>
        /// <param name="entity">갱신할 엔티티</param>
        Task UpdateAsync(string jwtToken, T command, CancellationToken cancellationToken);

        /// <summary>
        /// 엔티티를 삭제하고 서버에 해당 사실을 전송합니다.
        /// </summary>
        /// <param name="jwtToken">JWT 인증 토큰</param>
        /// <param name="id">삭제할 엔티티의 ID</param>
        Task DeleteAsync(string jwtToken, string id, T command, CancellationToken cancellationToken);
    }
    /// <summary>
    /// 클라이언트 측에서 CRUD 작업을 담당하는 서비스 클래스
    /// </summary>
    /// <typeparam name="T">작업 대상 엔티티 타입</typeparam>
    public class ClientCommandService<T> : ICommandService<T> where T : class, IRequest<bool>, ICommandLoggable
    {
        protected readonly ActorCommandContext _actorCommandContext;
        protected readonly CommandDbContext _commandContext;
        private readonly ILogger<ClientCommandService<T>> _logger;

        public ClientCommandService(ActorCommandContext actorCommandContext, CommandDbContext commandContext, ILogger<ClientCommandService<T>> logger)
        {
            _actorCommandContext = actorCommandContext;
            _commandContext = commandContext;
            _logger = logger;
        }
        /// <summary>
        /// 엔티티를 생성하고 서버에 전송합니다.
        /// </summary>
        /// <param name="jwtToken">JWT 인증 토큰</param>
        /// <param name="t">생성할 엔티티</param>
        public async Task CreateAsync(string jwtToken, T command, CancellationToken cancellationToken)
        {
            using var transaction = await _commandContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Command 객체를 CommandLog 객체로 변환
                CommandLog commandLog = command.ToCommandLog();

                // CommandLog 객체를 데이터베이스에 저장
                await _commandContext.CommandLogs.AddAsync(commandLog, cancellationToken);
                await _commandContext.SaveChangesAsync(cancellationToken);

                // HTTP 통신으로 서버에 명령 전송 (실제 명령 처리)
                var createResponse = await _actorCommandContext.Set<T>().PostAsync(jwtToken, command);
                if (!createResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("서버 전송 실패. 상태 코드: {StatusCode}", createResponse.StatusCode);
                    throw new Exception($"서버 전송 실패. 상태 코드: {createResponse.StatusCode}");
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("작업이 취소되었습니다. {Message}", ex.Message);
                await transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "명령 로깅 및 처리 중 예외 발생");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// 엔티티를 갱신하고 서버에 전송합니다.
        /// </summary>
        /// <param name="t">갱신할 엔티티</param>
        public async Task UpdateAsync(string jwtToken, T command, CancellationToken cancellationToken)
        {
            using var transaction = await _commandContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Command 객체를 CommandLog 객체로 변환
                CommandLog commandLog = command.ToCommandLog();

                // CommandLog 객체를 데이터베이스에 저장
                await _commandContext.CommandLogs.AddAsync(commandLog, cancellationToken);
                await _commandContext.SaveChangesAsync(cancellationToken);

                // HTTP 통신으로 서버에 명령 전송 (실제 명령 처리)
                var updateResponse = await _actorCommandContext.Set<T>().PutAsync(jwtToken, command);
                if (!updateResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("서버 전송 실패. 상태 코드: {StatusCode}", updateResponse.StatusCode);
                    throw new Exception($"서버 전송 실패. 상태 코드: {updateResponse.StatusCode}");
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("작업이 취소되었습니다. {Message}", ex.Message);
                await transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "명령 업데이트 중 예외 발생");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteAsync(string jwtToken, string id, T command, CancellationToken cancellationToken)
        {
            using var transaction = await _commandContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Command 객체를 CommandLog 객체로 변환
                CommandLog commandLog = command.ToCommandLog();

                // CommandLog 객체를 데이터베이스에 저장
                await _commandContext.CommandLogs.AddAsync(commandLog, cancellationToken);
                await _commandContext.SaveChangesAsync(cancellationToken);

                // HTTP 통신으로 서버에 삭제 명령 전송
                var deleteResponse = await _actorCommandContext.Set<T>().DeleteAsync(id, jwtToken);
                if (!deleteResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("서버 전송 실패. 상태 코드: {StatusCode}", deleteResponse.StatusCode);
                    throw new Exception($"서버 전송 실패. 상태 코드: {deleteResponse.StatusCode}");
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("작업이 취소되었습니다. {Message}", ex.Message);
                await transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "명령 삭제 중 예외 발생");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

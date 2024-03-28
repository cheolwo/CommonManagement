using FrontCommon.Actor;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;

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
        Task CreateAsync(string jwtToken, T entity, CancellationToken cancellationToken);


        /// <summary>
        /// 엔티티를 갱신하고 서버에 전송합니다.
        /// </summary>
        /// <param name="jwtToken">JWT 인증 토큰</param>
        /// <param name="entity">갱신할 엔티티</param>
        Task UpdateAsync(string jwtToken, T entity, CancellationToken cancellationToken );

        /// <summary>
        /// 엔티티를 삭제하고 서버에 해당 사실을 전송합니다.
        /// </summary>
        /// <param name="jwtToken">JWT 인증 토큰</param>
        /// <param name="id">삭제할 엔티티의 ID</param>
        Task DeleteAsync(string jwtToken, object id, CancellationToken cancellationToken);
    }
    /// <summary>
    /// 클라이언트 측에서 CRUD 작업을 담당하는 서비스 클래스
    /// </summary>
    /// <typeparam name="T">작업 대상 엔티티 타입</typeparam>
    public class ClientCommandService<T> : ICommandService<T> where T : class, IRequest<bool>
    {
        protected readonly ActorCommandContext _actorCommandContext;
        protected readonly DbContext _dbContext;
        private readonly ILogger<ClientCommandService<T>> _logger;

        public ClientCommandService(ActorCommandContext actorCommandContext, DbContext dbContext, ILogger<ClientCommandService<T>> logger)
        {
            _actorCommandContext = actorCommandContext;
            _dbContext = dbContext;     
            _logger = logger;
        }
        /// <summary>
        /// 엔티티를 생성하고 서버에 전송합니다.
        /// </summary>
        /// <param name="jwtToken">JWT 인증 토큰</param>
        /// <param name="t">생성할 엔티티</param>
        public async Task CreateAsync(string jwtToken, T t, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _dbContext.Set<T>().AddAsync(t, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var createResponse = await _actorCommandContext.Set<T>().PostAsync(jwtToken, t);
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
                _logger.LogError(ex, "엔티티 생성 중 예외 발생");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// 엔티티를 갱신하고 서버에 전송합니다.
        /// </summary>
        /// <param name="t">갱신할 엔티티</param>
        public async Task UpdateAsync(string jwtToken, T t, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // 로컬 DB에 갱신
                _dbContext.Set<T>().Update(t);
                await _dbContext.SaveChangesAsync();

                // 서버에 DTO 전송
                var updateResponse = await _actorCommandContext.Set<T>().PutAsync(jwtToken, t);
                if (!updateResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("서버 전송 실패. 상태 코드: {StatusCode}", updateResponse.StatusCode);
                    throw new Exception("서버 전송 실패");
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                // 취소 요청 시 롤백
                _logger.LogWarning("작업이 취소되었습니다. {Message}", ex.Message);
                await transaction.RollbackAsync(cancellationToken);
                // 필요한 취소 로직 처리
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "엔티티 생성 중 예외 발생");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteAsync(string jwtToken, object id, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // ID를 사용하여 로컬 DB에서 엔티티 조회
                var item = await _dbContext.Set<T>().FindAsync(id);
                if (item != null)
                {
                    // 로컬 DB에서 엔티티 삭제
                    _dbContext.Set<T>().Remove(item);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    // 서버에 삭제 요청 전송
                    var deleteResponse = await _actorCommandContext.Set<T>().
                                                    DeleteAsync(id.ToString(), jwtToken);
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        _logger.LogError("서버 전송 실패. 상태 코드: {StatusCode}", deleteResponse.StatusCode);
                        throw new Exception("서버 전송 실패. 엔티티 삭제 실패.");
                    }

                    // 트랜잭션 커밋
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException ex)
            {
                // 취소 요청 시 롤백
                _logger.LogWarning("작업이 취소되었습니다. {Message}", ex.Message);
                await transaction.RollbackAsync(cancellationToken);
                // 필요한 취소 로직 처리
            }
            catch (Exception ex)
            {
                // 예외 발생 시 트랜잭션 롤백
                _logger.LogError(ex, "엔티티 생성 중 예외 발생");
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

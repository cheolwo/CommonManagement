using DotNetCore.Repositories;
using System.Linq.Expressions;

namespace Common.Model.Repository
{
    public interface IEntityRepository<T> where T : class
    {
        // 서버로부터 데이터를 동기화하여 로컬 저장소에 업데이트하는 메서드
        Task SynchronizeAsync();

        // 특정 조건을 만족하는 데이터를 로컬 저장소에서 조회하는 메서드
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);

        // 데이터를 로컬 저장소에 추가하는 메서드
        new Task AddAsync(T entity);

        // 로컬 저장소에 있는 데이터를 업데이트하는 메서드
        new Task UpdateAsync(T entity);

        // 로컬 저장소에서 데이터를 삭제하는 메서드
        new Task DeleteAsync(T entity);
    }
}

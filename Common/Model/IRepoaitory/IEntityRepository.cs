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
    public interface IEntityQueryRepository<TEntity> : IQueryRepository<TEntity>
        where TEntity : Entity
    {
        Task<TEntity?> GetByCode(string code);
        Task<TEntity?> GetByName(string name);
    }
    public interface ICommodityQueryRepository<TEntity> : IEntityQueryRepository<TEntity>
        where TEntity : Commodity
    {
        Task<List<TEntity>> GetByQuantityGreaterThanAsync(string quantity);
        Task<List<TEntity>> GetByQuantityLessThanAsync(string quantity);
        Task<List<TEntity>> GetByQuantityBetweenAsync(string minValue, string maxValue);

    }
    public interface ICenterQueryRepository<TEntity> : IEntityQueryRepository<TEntity>
        where TEntity : Center
    {
        Task<TEntity?> GetByPhoneNumber(string phoneNumber);
        Task<TEntity?> GetByFaxNumber(string faxNumber);
        Task<TEntity?> GetByEmail(string email);
        Task<TEntity?> GetByAddress(string adress);
    }
    public interface IEntityCommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : class
    {
        Task SaveChangesAsync();
    }
    public interface IStatusCommandRepository<TEntity> : IEntityCommandRepository<TEntity>
            where TEntity : Status
    {

    }
    public interface ICenterCommandRepository<TEntity> : IEntityCommandRepository<TEntity> where TEntity : Center
    {

    }
    public interface ICommodityCommandRepository<TEntity> : IEntityCommandRepository<TEntity> where TEntity : Commodity
    {

    }
    public interface IStatusQueryRepository<TEntity> : IEntityCommandRepository<TEntity> where TEntity : Status
    {

    }
}

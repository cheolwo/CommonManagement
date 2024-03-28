using DotNetCore.Repositories;
using System.Linq.Expressions;

namespace Common.Model.Repository
{
    public interface IEntityRepository<T> where T : class
    {
        // �����κ��� �����͸� ����ȭ�Ͽ� ���� ����ҿ� ������Ʈ�ϴ� �޼���
        Task SynchronizeAsync();

        // Ư�� ������ �����ϴ� �����͸� ���� ����ҿ��� ��ȸ�ϴ� �޼���
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);

        // �����͸� ���� ����ҿ� �߰��ϴ� �޼���
        new Task AddAsync(T entity);

        // ���� ����ҿ� �ִ� �����͸� ������Ʈ�ϴ� �޼���
        new Task UpdateAsync(T entity);

        // ���� ����ҿ��� �����͸� �����ϴ� �޼���
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

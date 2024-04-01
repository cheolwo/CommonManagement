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
}

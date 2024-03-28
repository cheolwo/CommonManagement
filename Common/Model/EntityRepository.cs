using Common.Model.Repository;
using DotNetCore.EntityFrameworkCore;
using DotNetCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Common.Model
{
    public class EntityRepository<TEntity> : EFRepository<TEntity>
        where TEntity : Entity
    {
        protected readonly DbContext _dbContext;
        public EntityRepository(DbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<TEntity?> GetByCode(string code)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Code != null && e.Code.Equals(code));
        }

        public async Task<TEntity?> GetByName(string name)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Name != null && e.Name.Equals(name));
        }
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
    public class CommodityRepository<TEntity> : EntityRepository<TEntity>,
        ICommodityCommandRepository<TEntity>,
        ICommodityQueryRepository<TEntity> where TEntity : Commodity
    {
        public CommodityRepository(DbContext context) : base(context)
        {
        }
        public async Task<List<TEntity>> GetByQuantityGreaterThanAsync(string quantity)
        {
            return await _dbContext.Set<TEntity>()
                .Where(e => string.Compare(e.Quantity, quantity, StringComparison.OrdinalIgnoreCase) > 0)
                .ToListAsync();
        }

        public async Task<List<TEntity>> GetByQuantityLessThanAsync(string quantity)
        {
            return await _dbContext.Set<TEntity>()
                .Where(e => string.Compare(e.Quantity, quantity, StringComparison.OrdinalIgnoreCase) < 0)
                .ToListAsync();
        }

        public async Task<List<TEntity>> GetByQuantityBetweenAsync(string minValue, string maxValue)
        {
            return await _dbContext.Set<TEntity>()
                .Where(e => string.Compare(e.Quantity, minValue, StringComparison.OrdinalIgnoreCase) >= 0 &&
                            string.Compare(e.Quantity, maxValue, StringComparison.OrdinalIgnoreCase) <= 0)
                .ToListAsync();
        }
    }
    public class CenterRepository<TEntity> : EntityRepository<TEntity>,
        ICenterCommandRepository<TEntity>,
        ICenterQueryRepository<TEntity> where TEntity : Center
    {
        public CenterRepository(DbContext context) : base(context)
        {
        }

        public async Task<TEntity?> GetByAddress(string adress)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Address != null && e.Address.Equals(adress));
        }

        public async Task<TEntity?> GetByEmail(string email)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Email != null && e.Email.Equals(email));
        }

        public async Task<TEntity?> GetByFaxNumber(string faxNumber)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.FaxNumber != null && e.FaxNumber.Equals(faxNumber));
        }

        public async Task<TEntity?> GetByPhoneNumber(string phoneNumber)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.PhoneNumber != null && e.PhoneNumber.Equals(phoneNumber));
        }
    }
    public class StatusRepository<TEntity> : EntityRepository<TEntity>, IStatusQueryRepository<TEntity>,
                                IStatusCommandRepository<TEntity> where TEntity : Status
    {
        public StatusRepository(DbContext context) : base(context)
        {
        }
    }
}

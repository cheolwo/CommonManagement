//using Common.Actor;
//using FrontCommon.Actor;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System.Linq.Expressions;

//namespace Common.Model.Repository
//{
//    public class DesktopRepositoryManager<T> : IEntityRepository<T> where T : class
//    {
//        protected readonly ActorCommandContext _actorCommandContext;
//        protected readonly ActorQueryContext _actorQueryContext;
//        protected readonly DbContext _dbContext;
//        public DesktopRepositoryManager(ActorCommandContext actorCommandContext, ActorQueryContext actorQueryContext, DbContext dbContext)
//        {
//            _actorCommandContext = actorCommandContext;
//            _actorQueryContext = actorQueryContext;
//            _dbContext = dbContext;

//        }
//        public IQueryable<T> Queryable => throw new NotImplementedException();
//        public async Task AddAsync(T item)
//        {
//            var response = await _actorCommandContext.Set<T>().PostAsync(item);
//            // 2. HttpResponseMessage를 결과 값으로 받아 결과에 대해 읽음
//            if (response.IsSuccessStatusCode)
//            {
//                // 성공적으로 데이터가 서버에 추가된 경우
//                var responseBody = await response.Content.ReadAsStringAsync();
//                var updatedItem = JsonConvert.DeserializeObject<T>(responseBody);

//                // 3. 읽은 값을 로컬 DB에 갱신 (새로 추가하거나 업데이트)
//                var dbSet = _dbContext.Set<T>();
//                var entity = await dbSet.FindAsync(((dynamic)item).Id); // ID를 통해 엔티티 검색, item에 ID 프로퍼티가 있다고 가정
//                if (entity == null)
//                {
//                    await dbSet.AddAsync(updatedItem); // 로컬 DB에 새로 추가
//                }
//                else
//                {
//                    _dbContext.Entry(entity).CurrentValues.SetValues(updatedItem); // 기존 엔티티 업데이트
//                }
//                await _dbContext.SaveChangesAsync(); // 변경 사항 저장
//            }
//            else
//            {
//                // 에러 처리 로직 (예: 로그 남기기, 사용자에게 알림 등)
//                throw new Exception("Failed to add item to server.");
//            }
//        }

//        public Task DeleteAsync(T entity)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SynchronizeAsync()
//        {
//            throw new NotImplementedException();
//        }

//        public Task UpdateAsync(T entity)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
    

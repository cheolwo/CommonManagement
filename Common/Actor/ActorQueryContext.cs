using Common.Actor.Builder;
using Common.Actor.Builder.TypeBuilder;
using FrontCommon.Actor;
using MediatR;

namespace Common.Actor
{
    public class ActorQueryContext
    {
        protected readonly QueryBuilder dtoQueryBuilder = new();
        protected ActorQueryContext()
        {
            OnModelCreating(dtoQueryBuilder);
        }
        protected virtual void OnModelCreating(QueryBuilder dtoBuilder)
        {
        }
        public QueryTypeBuilder<TDto> Set<TDto>() where TDto : IRequest<TDto>
        {
            return dtoQueryBuilder.Set<TDto>();
        }
    }
}

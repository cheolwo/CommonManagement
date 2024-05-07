//using Common.Actor.Builder.TypeBuilder;
//using FrontCommon.Actor;
//using MediatR;

//namespace Common.Actor.Builder
//{
//    public class QueryBuilder
//    {
//        protected readonly Dictionary<Type, object> _configurations;
//        public void ApplyConfiguration<TDto>(IQueryConfiguration<TDto> configuration) where TDto : IRequest<TDto>
//        {
//            _configurations[typeof(TDto)] = configuration;
//        }

//        public QueryTypeBuilder<TDto> Set<TDto>() where TDto : IRequest<TDto>
//        {
//            return new QueryTypeBuilder<TDto>(
//                _configurations.ContainsKey(typeof(TDto)) ? (IQueryConfiguration<TDto>)_configurations[typeof(TDto)] : null);
//        }
//    }
//}

using FrontCommon.Actor;
using MediatR;

namespace Common.Actor.Builder
{
    public class DtoCommandBuilder : DtoBuilder
    {
        public void ApplyConfiguration<TDto>(ICommandConfiguration<TDto> configuration) where TDto : class, IRequest<bool>
        {
            _configurations[typeof(TDto)] = configuration;
        }
        public DtoTypeCommandBuilder<TDto> Set<TDto>() where TDto : class, IRequest<bool>
        {
            return new DtoTypeCommandBuilder<TDto>(_configurations.ContainsKey(typeof(TDto)) ? 
                (ICommandConfiguration<TDto>)_configurations[typeof(TDto)] : null);
        }
    }
}

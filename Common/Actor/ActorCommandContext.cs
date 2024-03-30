using Common.Actor.Builder;
using MediatR;

namespace FrontCommon.Actor
{
    public class ActorCommandContext
    {
        protected readonly CommandBuilder CommandBuilder = new();
        protected ActorCommandContext()
        {
            OnModelCreating(CommandBuilder);
        }

        protected virtual void OnModelCreating(CommandBuilder dtoBuilder) 
        {
           
        }
        public CommandTypeBuilder<TDto> Set<TDto>() where TDto : class, IRequest<bool>
        {
            return CommandBuilder.Set<TDto>();
        }
    }
}

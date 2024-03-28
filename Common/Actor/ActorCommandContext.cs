using Common.Actor.Builder;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FrontCommon.Actor
{
    public class ActorCommandContext
    {
        protected readonly IConfiguration _configuration;
        protected readonly CommandBuilder _commandBuilder = new();
        protected ActorCommandContext(IConfiguration configuration)
        {
            _configuration = configuration;
            OnModelCreating(_commandBuilder);
        }
        protected virtual void OnModelCreating(CommandBuilder commandBuilder) 
        {
           
        }
        public CommandTypeBuilder<TCommand> Set<TCommand>() where TCommand : IRequest<bool>
        {
            return _commandBuilder.Set<TCommand>();
        }
    }
}

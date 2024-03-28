using FrontCommon.Actor;
using MediatR;

namespace Common.Actor.Builder
{
    public class CommandBuilder
    {
        protected readonly Dictionary<Type, object> _configurations;
        public CommandBuilder() 
        {
            _configurations = [];
        }
        public void ApplyConfiguration<TCommand>(ICommandConfiguration<T> configuration) 
                                    where TCommand : IRequest<bool>
        {
            _configurations[typeof(TCommand)] = configuration;
        }
        public CommandTypeBuilder<TCommand> Set<TCommand>() where TCommand : IRequest<bool>
        {
            return new CommandTypeBuilder<TCommand>(_configurations.ContainsKey(typeof(TCommand)) ?
                (ICommandConfiguration<TCommand>)_configurations[typeof(TCommand)] : null);
        }
    }
}

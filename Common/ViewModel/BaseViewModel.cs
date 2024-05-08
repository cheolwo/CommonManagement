using FrontCommon.Actor;

namespace Common.ViewModel
{
    public class BaseViewModel
    {
        protected readonly ActorCommandContext _commandContext;
        public BaseViewModel(ActorCommandContext commandContext)
        {
            _commandContext = commandContext;
        }
    }
}

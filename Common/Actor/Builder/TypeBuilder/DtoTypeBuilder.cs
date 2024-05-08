using FrontCommon.Actor;
using MediatR;

namespace Common.Actor.Builder
{
    public class DtoTypeBuilder<TDto> where TDto : class, IRequest<bool>
    {
        protected List<ServerBaseRouteInfo> ServerBaseRoutes { get; } = new List<ServerBaseRouteInfo>();
        public DtoTypeBuilder<TDto> SetServerBaseRouteInfo(ServerBaseRouteInfo info)
        {
            ServerBaseRoutes.Add(info);
            return this;
        }
    }
}

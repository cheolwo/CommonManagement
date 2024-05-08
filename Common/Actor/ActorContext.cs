using Common.Actor.Builder;
using Common.Actor.Builder.TypeBuilder;
using MediatR;

namespace FrontCommon.Actor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CQRSAttribute : Attribute
    {
        public bool IsEnabled { get; }

        public CQRSAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
        public CQRSAttribute()
        {

        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DistributedAttribute : Attribute
    {
        public bool IsEnabled { get; }

        public DistributedAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
        public DistributedAttribute() 
        {
        }
    }

    public interface ICommandConfiguration<T> where T : class, IRequest<bool>
    {
        void Configure(CommandTypeBuilder<T> builder);
    }
    public interface IQueryConfiguration<T> where T : class, IRequest<T>
    {
        void Configure(QueryTypeBuilder<T> builder);
    }
    public class ServerBaseRouteInfo
    {
        public string Route { get; set; }
        public string BaseAddress { get; set; }
        public bool UseApiGateway { get; set; }
        public ServerBaseRouteInfo(string baseAddress, string route)
        {
            BaseAddress = baseAddress;
            Route = route;
        }
        public ServerBaseRouteInfo() { }
    }
}

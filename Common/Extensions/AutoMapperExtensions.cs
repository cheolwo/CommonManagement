using AutoMapper;
using System.Reflection;
using AutoMapper.Internal;
using AutoMapper.Configuration;

namespace Common.Extensions
{
    public static class AutoMapperExtensions
    {
        private static readonly PropertyInfo TypeMapActionsProperty = typeof(TypeMapConfiguration).GetProperty("TypeMapActions", BindingFlags.NonPublic | BindingFlags.Instance);

        // not needed in AutoMapper 12.0.1
        private static readonly PropertyInfo DestinationTypeDetailsProperty = typeof(TypeMap).GetProperty("DestinationTypeDetails", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void ForAllOtherMembers<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression, Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions)
        {
            var typeMapConfiguration = (TypeMapConfiguration)expression;

            var typeMapActions = (List<Action<TypeMap>>)TypeMapActionsProperty.GetValue(typeMapConfiguration);
            if (typeMapActions == null)
                throw new InvalidOperationException("TypeMapActions could not be retrieved.");

            typeMapActions.Add(typeMap =>
            {
                var destinationTypeDetails = (TypeDetails)DestinationTypeDetailsProperty.GetValue(typeMap);
                if (destinationTypeDetails == null)
                    throw new InvalidOperationException("DestinationTypeDetails could not be retrieved.");

                foreach (var accessor in destinationTypeDetails.WriteAccessors)
                {
                    if (typeMapConfiguration.GetDestinationMemberConfiguration(accessor) == null)
                    {
                        expression.ForMember(accessor.Name, memberOptions);
                    }
                }
            });
        }
    }
}

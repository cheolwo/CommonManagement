using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddSingleton(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var httpClient = httpClientFactory.CreateClient();
                return Activator.CreateInstance(typeof(TImplementation), new object[] { httpClient, configuration }) as TService;
            });
            return services;
        }
    }

}

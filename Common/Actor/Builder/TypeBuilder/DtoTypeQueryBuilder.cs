using FrontCommon.Actor;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;

namespace Common.Actor.Builder.TypeBuilder
{
    public class DtoTypeQueryBuilder<TDto> : DtoTypeBuilder<TDto> where TDto : class
    {
        public DtoTypeQueryBuilder(IDtoTypeQueryConfiguration<TDto> configuration)
        {
            configuration.Configure(this);
        }
        public async Task<List<TDto>?> GetToListAsync(string jwtToken)
        {
            var selectedRoute = GetSelectedBaseRoute();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(selectedRoute.BaseAddress);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await httpClient.GetAsync(selectedRoute.Route);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<List<TDto>>();
            }
        }
        public async Task<HttpResponseMessage> GetAsync(string jwtToken, TDto dto)
        {
            var selectedRoute = GetSelectedBaseRoute();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(selectedRoute.BaseAddress);

                // Set Authorization header with JWT token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                var jsonContent = JsonConvert.SerializeObject(dto);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                return await httpClient.PostAsync(selectedRoute.Route, httpContent);
            }
        }
        private ServerBaseRouteInfo GetSelectedBaseRoute()
        {
            var IsCqrs = IsApiGatewayCompatible();
            if (IsCqrs)
            {
                // DTO에 CQRS 특성이 있고 활성화된 경우 API Gateway를 사용하는 서버 선택
                var selectedRoute = ServerBaseRoutes.FirstOrDefault(route => route.UseApiGateway);
                if (selectedRoute != null)
                {
                    return selectedRoute;
                }
            }

            // CQRS 특성이 없거나 비활성화된 경우 비즈니스 서버 선택
            var defaultRoute = ServerBaseRoutes.FirstOrDefault();
            if (defaultRoute != null)
            {
                return defaultRoute;
            }

            throw new Exception("No serve base route available.");
        }

        private bool IsApiGatewayCompatible()
        {
            var cqsAttribute = typeof(TDto).GetCustomAttribute<CQRSAttribute>();
            return cqsAttribute != null && cqsAttribute.IsEnabled;
        }
    }

}

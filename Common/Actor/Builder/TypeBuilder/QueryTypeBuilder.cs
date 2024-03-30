using FrontCommon.Actor;
using MediatR;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;

namespace Common.Actor.Builder.TypeBuilder
{
    public class QueryTypeBuilder<TDto> where TDto : IRequest<TDto>
    {
        public QueryTypeBuilder(IQueryConfiguration<TDto> configuration)
        {
            configuration.Configure(this);
        }
        public async Task<List<TDto>?> GetToListAsync(string jwtToken)
        {
            var selectedRoute = GetSelectedBaseRoute();
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(selectedRoute.BaseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await httpClient.GetAsync(selectedRoute.Route);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<TDto>>();
        }
        // Overload to accept query parameters as a Dictionary
        public async Task<List<TDto>?> GetWithQueryAsync(string jwtToken, Dictionary<string, string> queryParams)
        {
            var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
            var requestUri = $"{GetSelectedBaseRoute().Route}?{queryString}";
            using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<TDto>>();
        }
        // ID를 사용하여 단일 객체를 가져오는 메서드 추가
        public async Task<TDto?> GetAsync(string jwtToken, string id)
        {
            var selectedRoute = GetSelectedBaseRoute();
            var requestUri = $"{selectedRoute.BaseAddress}{selectedRoute.Route}/{id}"; // ID를 URL에 포함
            using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TDto>();
        }
        public async Task<HttpResponseMessage> GetAsync(string jwtToken, TDto dto)
        {
            var selectedRoute = GetSelectedBaseRoute();

            using var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(selectedRoute.BaseAddress);

            // Set Authorization header with JWT token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var jsonContent = JsonConvert.SerializeObject(dto);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(selectedRoute.Route, httpContent);
        }
        // Overload to accept custom headers
        public async Task<HttpResponseMessage> GetAsync(string jwtToken, TDto dto, Dictionary<string, string> headers)
        {
            var requestUri = GetSelectedBaseRoute().Route;
            using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            foreach (var header in headers)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var jsonContent = JsonConvert.SerializeObject(dto);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync(requestUri, httpContent);
        }
        private bool IsApiGatewayCompatible()
        {
            var cqsAttribute = typeof(TDto).GetCustomAttribute<CQRSAttribute>();
            return cqsAttribute != null && cqsAttribute.IsEnabled;
        }
    }

}

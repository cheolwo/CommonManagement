using FluentValidation;
using FrontCommon.Actor;
using MediatR;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Common.Actor.Builder
{
    public class CommandTypeBuilder<TCommand> where TCommand : IRequest<bool>
    {
        protected IValidator<TCommand> Validator { get; private set; }
        protected string Route { get; private set; }
        protected string BaseAddress { get; private set; }

        public CommandTypeBuilder(ICommandConfiguration<TCommand> configuration)
        {
            configuration.Configure(this);
        }
        public CommandTypeBuilder<TCommand> SetServerBaseAddress(string baseAddress)
        {
            Route = baseAddress;
            return this;
        }

        public CommandTypeBuilder<TCommand> SetValidator(IValidator<TCommand> validator)
        {
            Validator = validator;
            return this;
        }
        public IValidator<TCommand> GetValidator()
        {
            return Validator;
        }
        public CommandTypeBuilder<TCommand> SetRoute(string route)
        {
            Route = route;
            return this;
        }

        public CommandTypeBuilder<TCommand> ApplyConfiguration(
            ICommandConfiguration<TCommand> configuration)
        {
            configuration.Configure(this);
            return this;
        }
        public async Task<HttpResponseMessage> SendRequestAsync(
            HttpMethod method, string jwtToken, TCommand command, string? id = null)
        {
            if (Validator != null)
            {
                var validationResult = await Validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new Exception("command validation failed: " + string.Join(", ", validationResult.Errors));
                }
            }

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseAddress);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            HttpRequestMessage request = new(method, method == HttpMethod.Delete ? $"{Route}/{id}" : Route);

            if (method != HttpMethod.Delete)
            {
                var jsonContent = JsonConvert.SerializeObject(command);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            }

            return await httpClient.SendAsync(request);
        }
        /// <summary>
        /// _ActorContext.Set<TCommand>().PostAsync(jwtToken, command);
        /// </summary>
        public async Task<HttpResponseMessage> PostAsync(string jwtToken, TCommand command)
        {
            if (Validator != null)
            {
                var validationResult = await Validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new Exception("command validation failed: " + string.Join(", ", validationResult.Errors));
                }
            }

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseAddress);

            // Set Authorization header with JWT token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var jsonContent = JsonConvert.SerializeObject(command);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            
            return await httpClient.PostAsync(Route, httpContent);
        }
        public async Task<HttpResponseMessage> PutAsync(string jwtToken, TCommand command)
        {
            if (Validator != null)
            {
                var validationResult = await Validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new Exception("command validation failed: " + string.Join(", ", validationResult.Errors));
                }
            }

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseAddress);

            // Set Authorization header with JWT token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var jsonContent = JsonConvert.SerializeObject(command);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await httpClient.PutAsync(BaseAddress, httpContent);
        }
        public async Task<HttpResponseMessage> DeleteAsync(string id, string jwtToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseAddress);

                // Set Authorization header with JWT token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                return await httpClient.DeleteAsync($"{Route}/{id}");
            }
        }
        public async Task<HttpResponseMessage> PostAsync(TCommand command)
        {
            if (Validator != null)
            {
                var validationResult = await Validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new Exception("command validation failed: " + string.Join(", ", validationResult.Errors));
                }
            }

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseAddress);
            var jsonContent = JsonConvert.SerializeObject(command);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(Route, httpContent);
        }
        public async Task<HttpResponseMessage> PutAsync(TCommand command)
        {
            if (Validator != null)
            {
                var validationResult = await Validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    throw new Exception("command validation failed: " + string.Join(", ", validationResult.Errors));
                }
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseAddress);;
                var jsonContent = JsonConvert.SerializeObject(command);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                return await httpClient.PutAsync(BaseAddress, httpContent);
            }
        }
        public async Task<HttpResponseMessage> DeleteAsync(string id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseAddress);
                return await httpClient.DeleteAsync($"{Route}/{id}");
            }
        }
        private bool IsApiGatewayCompatible(TCommand command)
        {
            var cqsAttribute = command.GetType().GetCustomAttribute<CQRSAttribute>();
            return cqsAttribute != null && cqsAttribute.IsEnabled;
        }
    }
}

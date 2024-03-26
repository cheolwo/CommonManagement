using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace Common.MiddleWare
{
    public class HMACVerificationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _sharedSecretKey = "YourSharedSecretKey";

        public HMACVerificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // HMAC 검증
            string receivedHmac = context.Request.Headers["X-HMAC"];
            string requestBody;

            using (var reader = new StreamReader(context.Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody)); // Body를 다시 읽을 수 있도록 재설정
            }

            string calculatedHmac = GenerateHmac(requestBody, _sharedSecretKey);

            if (calculatedHmac != receivedHmac)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("HMAC validation failed.");
                return;
            }

            // Token 검증 및 역할 확인 로직 (생략)

            await _next(context);
        }

        private string GenerateHmac(string data, string key)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hashmessage);
            }
        }
    }

    public static class VerificationMiddlewareExtensions
    {
        public static IApplicationBuilder UseHMACVerificationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HMACVerificationMiddleware>();
        }
    }

}

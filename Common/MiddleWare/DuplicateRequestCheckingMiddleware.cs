using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MiddleWare
{
    public class DuplicateRequestCheckingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public DuplicateRequestCheckingMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var requestHash = GenerateHash(requestBody);
            var cacheKey = $"RequestHash:{requestHash}";

            // Redis에서 해시값 검사
            if (await _cache.GetStringAsync(cacheKey) != null)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsync("Duplicate request");
                return;
            }

            // 요청 해시를 Redis에 저장 (예: 30초 동안 유지)
            await _cache.SetStringAsync(cacheKey, "exists", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

            // 요청 본문을 스트림으로 복원
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            await _next(context);
        }

        private string GenerateHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }
    }

}

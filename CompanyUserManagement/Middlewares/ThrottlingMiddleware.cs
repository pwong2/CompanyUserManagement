using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace CompanyUserManagement.Middlewares
{
    public class ThrottlingMiddleware
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        private readonly RequestDelegate _next;

        public ThrottlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var userId = context.User.FindFirst(ClaimTypes.Name)?.Value;

                if (userId != null)
                {
                    var cacheKey = $"RequestCount_{userId}";
                    var requestCount = Cache.Get<int?>(cacheKey) ?? 0;

                    if (requestCount >= 10)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsync("Request limit exceeded. Try again later.");
                        return;
                    }

                    Cache.Set(cacheKey, requestCount + 1, DateTimeOffset.UtcNow.AddMinutes(1));
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
            }
        }
    }
}

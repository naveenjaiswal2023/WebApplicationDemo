using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Filters
{
    public class ResponseCacheFilter : IActionFilter
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public ResponseCacheFilter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var cacheKey = context.HttpContext.Request.Path;
            if (_cache.TryGetValue(cacheKey, out var cachedResponse))
            {
                context.Result = new ObjectResult(cachedResponse);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var cacheKey = context.HttpContext.Request.Path;
                _cache.Set(cacheKey, objectResult.Value, _cacheDuration);
            }
        }
    }
}

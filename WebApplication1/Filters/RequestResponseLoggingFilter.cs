using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Text.Json;

namespace WebApplication1.Filters
{
    public class RequestResponseLoggingFilter : IActionFilter
    {
        private readonly ILogger<RequestResponseLoggingFilter> _logger;
        private Stopwatch _stopwatch;

        public RequestResponseLoggingFilter(ILogger<RequestResponseLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            var request = context.HttpContext.Request;
            _logger.LogInformation($"Incoming request: {request.Method} {request.Path}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var response = new
            {
                StatusCode = context.HttpContext.Response.StatusCode,
                ExecutionTime = $"{_stopwatch.ElapsedMilliseconds} ms"
            };

            _logger.LogInformation($"Response: {JsonSerializer.Serialize(response)}");
        }

    }
}

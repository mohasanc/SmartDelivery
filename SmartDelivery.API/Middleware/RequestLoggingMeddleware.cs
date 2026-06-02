using System.Diagnostics;

namespace SmartDelivery.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation(
                    "➜ {Method} {Path}{QueryString} | IP: {IP} | TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.Connection.RemoteIpAddress,
                    context.TraceIdentifier
                );

                await _next(context);
            }
            finally
            {
                sw.Stop();

                var level = context.Response.StatusCode >= 500
                ? LogLevel.Error
                : context.Response.StatusCode >= 400
                    ? LogLevel.Warning
                    : LogLevel.Information;

                _logger.Log(level,
                "✔ {Method} {Path} → {StatusCode} in {ElapsedMs}ms | TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds,
                context.TraceIdentifier);
            }
        }
    }
}

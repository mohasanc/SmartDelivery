using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace SmartDelivery.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception on {Method} {Path} | TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                ArgumentNullException => (HttpStatusCode.BadRequest, "A required value was null."),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized."),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found."),
                DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Data conflict detected. Please refresh and try again."),
                DbUpdateException => (HttpStatusCode.InternalServerError, "A database error occurred."),
                OperationCanceledException => (HttpStatusCode.ServiceUnavailable, "The operation was cancelled."),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new ApiErrorResponse(
                StatusCode: (int)statusCode,
                Message: message,
                TraceId: context.TraceIdentifier,
                Errors: null
            );

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
    public record ApiErrorResponse(
        int StatusCode,
        string Message,
        string TraceId,
        IEnumerable<string>? Errors
    );
}

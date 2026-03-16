using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace InfinityGrowth_Proyecto2
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

                _logger.LogError(
                    ex,
                    "Unhandled exception. TraceId: {TraceId}. Path: {Path}. Method: {Method}",
                    traceId,
                    context.Request.Path.Value,
                    context.Request.Method
                );

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = MapStatusCode(ex);

                var response = new ErrorResponse
                {
                    Message = "An unexpected error occurred.",
                    TraceId = traceId
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

        private static int MapStatusCode(Exception ex)
        {
            return ex switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private sealed class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
            public string TraceId { get; set; } = string.Empty;
        }
    }
}


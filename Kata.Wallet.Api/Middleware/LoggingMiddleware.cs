using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Resources;

namespace Kata.Wallet.Api.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly ResourceManager _resourceManager;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, ResourceManager resourceManager)
        {
            _next = next;
            _logger = logger;
            _resourceManager = resourceManager;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            try
            {
                _logger.LogInformation($"[REQUEST] {request.Method} {request.Path} | Query: {JsonSerializer.Serialize(request.Query)}");

                await _next(context);

                stopwatch.Stop();
                _logger.LogInformation($"[RESPONSE] {context.Response.StatusCode} | Duration: {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"[ERROR] {request.Method} {request.Path} | Duration: {stopwatch.ElapsedMilliseconds}ms");

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    message = _resourceManager.GetString("Error_General"),
                    statusCode = context.Response.StatusCode
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }

}

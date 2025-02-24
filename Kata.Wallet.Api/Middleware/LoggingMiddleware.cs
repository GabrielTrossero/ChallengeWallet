using System.Diagnostics;
using System.Text.Json;

namespace Kata.Wallet.Api.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            _logger.LogInformation($"[REQUEST] {request.Method} {request.Path} | Query: {JsonSerializer.Serialize(request.Query)}");

            if (request.ContentLength > 0 && request.Body.CanSeek)
            {
                request.Body.Seek(0, System.IO.SeekOrigin.Begin);
                using var reader = new System.IO.StreamReader(request.Body);
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation($"[REQUEST BODY] {body}");
                request.Body.Seek(0, System.IO.SeekOrigin.Begin);
            }

            var originalResponseBody = context.Response.Body;
            using var newResponseBody = new System.IO.MemoryStream();
            context.Response.Body = newResponseBody;

            await _next(context);

            stopwatch.Stop();

            newResponseBody.Seek(0, System.IO.SeekOrigin.Begin);
            var responseBody = await new System.IO.StreamReader(newResponseBody).ReadToEndAsync();
            _logger.LogInformation($"[RESPONSE] {context.Response.StatusCode} | Duration: {stopwatch.ElapsedMilliseconds}ms | Body: {responseBody}");

            newResponseBody.Seek(0, System.IO.SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;
        }
    }
}

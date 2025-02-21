using Serilog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.API.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
       
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;           
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Process the request
            await _next(context);

            stopwatch.Stop();

            var clientIp = GetClientIp(context);
            var clientId = context.User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;
            var method = context.Request.Method;
            var endpoint = context.Request.Path;
            var responseCode = context.Response.StatusCode;
            var responseTime = stopwatch.ElapsedMilliseconds;

            Log.Information("Request details: Client IP: {ClientIp}, ClientId: {ClientId}, HTTP Method: {Method}, Target Endpoint: {Endpoint}, Response Code: {ResponseCode}, Response Time: {ResponseTime}ms",
                clientIp, clientId, method, endpoint, responseCode, responseTime);
        }
        private static string? GetClientIp(HttpContext context)
        {
            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}

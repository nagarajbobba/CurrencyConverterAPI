using CurrencyConverter.API.Configuration.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.API.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string _correlationIdHeader = "X-Correlation-Id";
        // Constructor to initialize middleware with RequestDelegate and IConfiguration
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            // Get or generate correlation ID
            var correlationId = GetCorrelationIdTrace(context, correlationIdGenerator);

            // If the correlation ID is not present in the request header, generate a new one
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }
            // Add correlation ID to response headers
            AddCorrelationIdToResponse(context, correlationId);

            // Call the next middleware in the pipeline
            await _next(context);
        }


        // Method to get or generate correlation ID from request
        private static string GetCorrelationIdTrace(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            if (context == null || correlationIdGenerator == null) return string.Empty;

            // Check if correlation ID exists in request headers
            if (context.Request.Headers.TryGetValue(_correlationIdHeader, out var correlationId))
            {
                correlationIdGenerator.Set(correlationId!);
                return correlationId!;
            }
            else
            {
                return correlationIdGenerator.Get();
            }
        }
        // Method to add correlation ID to response headers
        private static void AddCorrelationIdToResponse(HttpContext context, string correlationId)
        {
            if (string.IsNullOrWhiteSpace(correlationId)) return;
            if (!context.Request.Headers.ContainsKey(_correlationIdHeader))
            {
                context.Request.Headers[_correlationIdHeader] = correlationId;
            }
            context.Response.OnStarting(() =>
            {
                // Check if correlation ID already exists in response headers
                if (context.Response.Headers[_correlationIdHeader].ToString() is not null)
                {
                    context.Response.Headers[_correlationIdHeader] = new[] { correlationId };
                }
                else
                {
                    if (context.Response.Headers.ContainsKey(_correlationIdHeader))
                    {
                        context.Response.Headers[_correlationIdHeader] = correlationId;
                    }
                    else
                    {
                        context.Response.Headers.Append(_correlationIdHeader, correlationId);
                    }
                }
                return Task.CompletedTask;
            });
        }
    }
}

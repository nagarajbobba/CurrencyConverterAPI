using FluentValidation;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace CurrencyConverter.API.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // TODO: 
            //context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId);
            //var correlationId = context.TraceIdentifier;
            try
            {
                await _next(context);
            }
            catch(ValidationException ex)
            {
                context.Response.Headers.TryGetValue("X-Correlation-Id", out var correlationId);
                Log.Error(ex, "Validation Failed. CorrelationId: {CorrelationId}", correlationId);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(new
                {
                    context.Response.StatusCode,
                    ex.Message
                }.ToString() ?? string.Empty);
            }
            catch (Exception ex)
            {
                context.Response.Headers.TryGetValue("X-Correlation-Id", out var correlationId);
                Log.Error(ex, "An unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);
                await HandleExceptionAsync(context);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new
            {
                context.Response.StatusCode,
                Message = "Internal Server Error"
            }.ToString() ?? string.Empty);
        }
    }
}

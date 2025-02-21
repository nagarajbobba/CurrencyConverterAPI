using AspNetCoreRateLimit;
using CurrencyConverter.API.Configuration;
using CurrencyConverter.API.Configuration.Interfaces;
using CurrencyConverter.API.JwtAuthentication;
using CurrencyConverter.API.JwtAuthentication.Interfaces;
using CurrencyConverter.API.JwtAuthentication.Services;
using CurrencyConverter.API.Middlewares;
using CurrencyConverter.Application;
using CurrencyConverter.Catching;
using CurrencyConverter.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using MassTransit.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRedisCacheProvider(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddHttpClient();

#region OpenTelemetry
// Configure OpenTelemetry
//builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
//{
//    tracerProviderBuilder
//        .SetResourceBuilder(ResourceBuilder.CreateDefault()
//            .AddService("CurrencyConverterAPI"))
//        .AddAspNetCoreInstrumentation()  // Instrument ASP.NET Core requests
//        .AddHttpClientInstrumentation()  // Instrument outgoing HTTP requests
//        .AddJaegerExporter(options =>    // Export traces to Jaeger
//        {
//            options.AgentHost = builder.Configuration["Jaeger:Host"]; // Jaeger host
//            options.AgentPort = int.Parse(builder.Configuration["Jaeger:Port"]); // Jaeger port (default: 6831)
//        });
//});
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("CurrencyConverterAPI"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(DiagnosticHeaders.DefaultListenerName)
       .AddJaegerExporter(options =>
        {
            options.AgentHost = builder.Configuration["Jaeger:Host"]; // Jaeger host
            options.AgentPort = int.Parse(builder.Configuration["Jaeger:Port"]); // Jaeger port (default: 6831)
            options.ExportProcessorType = ExportProcessorType.Simple; // Use Simple processor for debugging

        });
    });
#endregion OpenTelemetry
#region ApiRateLimiting
// Load configuration from appsettings.json
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

// Inject the rate limit services
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();
#endregion ApiRateLimiting

builder.Services.RegisterDependencies(builder.Configuration);
builder.Services.RegisterApplicationDependencies(); 

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.Configure<ExchangeSettings>(builder.Configuration.GetSection(nameof(ExchangeSettings)));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<ITokenService, TokenService>();

#region CorrelationId
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
#endregion CorrelationId

#region Serilogger
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("CorrelationId", "{CorrelationId}")
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.File("logs/request_log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
#endregion Serilogger

#region API Versioning
// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("X-Api-version"),
    new UrlSegmentApiVersionReader());
});
/// Add ApiExplorer to discover versions
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

#endregion API Versioning

var app = builder.Build();


// Register the request logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// error handling middleware
app.UseMiddleware<ExceptionMiddleware>();

// Register the correlation ID middleware
app.UseMiddleware<CorrelationIdMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Enable rate limiting middleware
app.UseIpRateLimiting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
// Use middleware to validate JWT tokens
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

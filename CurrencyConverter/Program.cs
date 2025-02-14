using CurrencyConverter.API.Configuration;
using CurrencyConverter.API.JwtAuthentication;
using CurrencyConverter.API.JwtAuthentication.Interfaces;
using CurrencyConverter.API.JwtAuthentication.Services;
using CurrencyConverter.API.Middlewares;
using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetLatestRatesQuery).Assembly));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddHttpClient();

// Registers the ReadRepository from Infrastucture
builder.Services.RegisterDependencies();

//builder.Host.UseSerilog((context, config) =>
//{
//    config.WriteTo.Console().ReadFrom.Configuration(context.Configuration);
//});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// error handling middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Use middleware to validate JWT tokens
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();

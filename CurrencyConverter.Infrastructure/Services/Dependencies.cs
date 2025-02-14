using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Infrastructure.CurrencyConverter;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Infrastructure.Services
{
    public static class Dependencies
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyConverterReadRepository, CurrencyConverterReadRepository>();
            return services;
        }
    }
}

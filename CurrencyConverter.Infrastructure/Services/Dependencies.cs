using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Infrastructure.CurrencyConverter;
using CurrencyConverter.Infrastructure.Utility;
using CurrencyConverter.Infrastructure.Utility.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Infrastructure.Services
{
    public static class Dependencies
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services, IConfigurationManager configuration)
        {
            var exchangeSettings = new ExchangeSettings();
            configuration.GetSection(nameof(ExchangeSettings)).Bind(exchangeSettings);
            services.AddSingleton<IExchangeSettings>(exchangeSettings);
            services.AddSingleton<ICacheHelper, CacheHelper>();
            services.AddScoped<IHttpClientExtensions, HttpClientExtensions>();
            services.AddScoped<FrankfurterApiProviderReadRepository>();
            services.AddScoped<AlternateApiProviderReadRepository>();
            services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();
            //services.AddScoped<ICurrencyProviderReadRepository, FrankfurterApiProviderReadRepository>();
            return services;
        }
    }
}

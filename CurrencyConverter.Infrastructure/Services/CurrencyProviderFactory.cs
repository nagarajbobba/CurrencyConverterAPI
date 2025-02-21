using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Infrastructure.CurrencyConverter;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Infrastructure.Services
{
    public class CurrencyProviderFactory : ICurrencyProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrencyProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ICurrencyProviderReadRepository GetProvider(string providerType)
        {
            return providerType switch
            {
                "FrankFurterApiProvider" => _serviceProvider.GetRequiredService<FrankfurterApiProviderReadRepository>(),
                "AnotherExchangeRatesApiProvider" => _serviceProvider.GetRequiredService<AlternateApiProviderReadRepository>(),
                _ => throw new NotImplementedException()
            };
        }
    }
}

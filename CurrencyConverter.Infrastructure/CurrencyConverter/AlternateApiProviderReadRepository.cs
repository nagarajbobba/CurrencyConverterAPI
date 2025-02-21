using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Core.Entities;

namespace CurrencyConverter.Infrastructure.CurrencyConverter
{
    public class AlternateApiProviderReadRepository : ICurrencyProviderReadRepository
    {
        public Task<CurrencyConversionResponse> ConvertAsync(CurrencyConversionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CurrencyRatesHistoryResponse> GetHistoricalRatesAsync(CurrencyRatesHistoryRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CurrencyRates> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Core.Entities;

namespace CurrencyConverter.Application.Currency.Interfaces
{
    public interface ICurrencyConverterReadRepository
    {
        Task<CurrencyRates> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken);
        Task<CurrencyConversionResponse> ConvertAsync(CurrencyConversionRequest request, CancellationToken cancellationToken);
        Task<CurrencyRatesHistoryResponse> GetHistoricalRatesAsync(CurrencyRatesHistoryRequest request, CancellationToken cancellationToken);

    }
}

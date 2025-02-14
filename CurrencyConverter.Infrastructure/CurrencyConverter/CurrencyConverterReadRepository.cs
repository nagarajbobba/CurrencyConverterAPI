using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Core.Entities;
using System.Text.Json;

namespace CurrencyConverter.Infrastructure.CurrencyConverter
{
    public class CurrencyConverterReadRepository : ICurrencyConverterReadRepository
    {
        private readonly HttpClient _httpClient;

        public CurrencyConverterReadRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CurrencyConversionResponse> ConvertAsync(CurrencyConversionRequest request, CancellationToken cancellationToken)
        {
            //var result = await _httpClient.GetAsync($"https://api.frankfurter.dev/v1/latest?base={request.BaseCurrency}&symbols={request.TargetCurrency}", cancellationToken);
            var result = await _httpClient.GetAsync($"https://api.frankfurter.dev/v1/latest?base={request.BaseCurrency}&symbols={request.TargetCurrency}", cancellationToken);
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync(cancellationToken);
                var currencyRates = JsonSerializer.Deserialize<CurrencyRates>(content);
                if (currencyRates != null && currencyRates.Rates.ContainsKey(request.TargetCurrency))
                {
                    var convertedAmount = request.Amount * currencyRates.Rates[request.TargetCurrency];
                    return new CurrencyConversionResponse
                    {
                        BaseCurrency = request.BaseCurrency,
                        TargetCurrency = request.TargetCurrency,
                        Amount = request.Amount,
                        ConvertedAmount = convertedAmount
                    };
                }
                else
                {
                    throw new InvalidOperationException("Failed to deserialize currency rates or target currency not found.");
                }
            }
            else
            {
                throw new HttpRequestException($"Request to convert currency failed with status code {result.StatusCode}.");
            }
        }

        public async Task<CurrencyRatesHistoryResponse> GetHistoricalRatesAsync(CurrencyRatesHistoryRequest request, CancellationToken cancellationToken)
        {
            var url = $"https://api.frankfurter.app/{request.StartDate}..{request.EndDate}?base={request.BaseCurrency}";
            var result = await _httpClient.GetAsync(url, cancellationToken);
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync(cancellationToken);
                var historydata = JsonSerializer.Deserialize<CurrencyRatesHistoryResponse>(content);
                if (historydata != null)
                {
                    return historydata;
                }
                else
                {
                    throw new InvalidOperationException("Failed to deserialize currency rates");
                }
            }
            else
            {
                throw new HttpRequestException($"Request to get history data failed with status code {result.StatusCode}.");
            }

        }

        public async Task<CurrencyRates> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _httpClient.GetAsync($" https://api.frankfurter.app/latest?base={baseCurrency}", cancellationToken);
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync(cancellationToken);
                    var currencyRates = JsonSerializer.Deserialize<CurrencyRates>(content);
                    if (currencyRates != null)
                    {
                        return currencyRates;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to deserialize currency rates.");
                    }
                }
                else
                {
                    throw new HttpRequestException($"Request to get latest rates failed with status code {result.StatusCode}.");
                }
            }
            catch (Exception)
            {
                throw;
            }
           
            //if (result.IsSuccessStatusCode)
            //{
            //    var content = await result.Content.ReadAsStringAsync();
            //    return JsonSerializer.Deserialize<CurrencyRates>(content);
            //}
            //return null;
        }
    }
}

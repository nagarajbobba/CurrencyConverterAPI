using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Core.Entities;
using CurrencyConverter.Infrastructure.Utility;
using CurrencyConverter.Infrastructure.Utility.Interfaces;
using Polly;
using Polly.CircuitBreaker;
using System.Text.Json;

namespace CurrencyConverter.Infrastructure.CurrencyConverter
{
    public class FrankfurterApiProviderReadRepository : ICurrencyProviderReadRepository
    {     
        private readonly IExchangeSettings _exchangeSettings;       
        private readonly ICacheHelper _cacheHelper;
        private readonly IHttpClientExtensions _httpClientExtensions;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;


        public FrankfurterApiProviderReadRepository(IExchangeSettings exchangeSettings,
            ICacheHelper cacheHelper,
            IHttpClientExtensions httpClientExtensions)
        {
            _exchangeSettings = exchangeSettings;            
            _cacheHelper = cacheHelper;
            _httpClientExtensions = httpClientExtensions;

            // Initialize the circuit breaker policy
            _circuitBreakerPolicy = CircuitBreakerPolicyProvider.GetCircuitBreakerPolicy();
        }
        private async Task<HttpResponseMessage> ExecuteWithCircuitBreakerAsync(Func<Task<HttpResponseMessage>> action)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(action);
        }
        public async Task<CurrencyConversionResponse> ConvertAsync(CurrencyConversionRequest request, CancellationToken cancellationToken)
        {
           
            var url = _exchangeSettings.FrankFurterApiProvider + $"latest?base={request.BaseCurrency}&symbols={request.TargetCurrency}";

            /* Executes an HTTP GET request to the specified URL with a retry policy.
            Retries the request up to 3 times with an exponential backoff in case of transient failures.*/
            var result = await ExecuteWithCircuitBreakerAsync(() => _httpClientExtensions.GetWithRetryAsync(url, cancellationToken));
            
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
            // Retrieve the data from cache if it exists
            var key = $"{request.StartDate}-{request.EndDate}-{request.BaseCurrency}-{request.Page}-{request.PageSize}";
            var historyRates = await _cacheHelper.GetAsync<CurrencyRatesHistoryResponse>(key);
            if (historyRates != null)
            {
                return historyRates.Value;
            }

            var (startDate, endDate) = DateRangeCalculator.GetDateRangeForPagination(Convert.ToDateTime(request.StartDate), Convert.ToDateTime(request.EndDate), request.PageSize, request.Page);

            var url = _exchangeSettings.FrankFurterApiProvider + $"{startDate}..{endDate}?base={request.BaseCurrency}";

            /* Executes an HTTP GET request to the specified URL with a retry policy.
             Retries the request up to 3 times with an exponential backoff in case of transient failures.*/
            var result = await ExecuteWithCircuitBreakerAsync(() => _httpClientExtensions.GetWithRetryAsync(url, cancellationToken));
            if (result.IsSuccessStatusCode)
            {               
                var content = await result.Content.ReadAsStringAsync(cancellationToken);
                var historydata = JsonSerializer.Deserialize<CurrencyRatesHistoryResponse>(content);
                if (historydata != null)
                {
                    await _cacheHelper.SetAsync<CurrencyRatesHistoryResponse>(key, historydata);
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
            // Retrieve the data from cache if it exists
            var latestRates = await _cacheHelper.GetAsync<CurrencyRates>(baseCurrency);
            if (latestRates != null)
                return latestRates.Value;

            var url = _exchangeSettings.FrankFurterApiProvider + $"latest?base={baseCurrency}";

            /* Executes an HTTP GET request to the specified URL with a retry policy.
            Retries the request up to 3 times with an exponential backoff in case of transient failures.*/
            var result = await ExecuteWithCircuitBreakerAsync(() => _httpClientExtensions.GetWithRetryAsync(url, cancellationToken));


            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync(cancellationToken);
                var currencyRates = JsonSerializer.Deserialize<CurrencyRates>(content);
                if (currencyRates != null)
                {
                    await _cacheHelper.SetAsync<CurrencyRates>(baseCurrency, currencyRates);                   

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
       
    }
}

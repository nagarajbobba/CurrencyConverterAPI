using CurrencyConverter.Infrastructure.Utility.Interfaces;
using Polly;

namespace CurrencyConverter.Infrastructure.Utility
{
    public class HttpClientExtensions : IHttpClientExtensions
    {
        private readonly HttpClient _httpClient;
        public HttpClientExtensions(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<HttpResponseMessage> GetWithRetryAsync(string url, CancellationToken cancellationToken, int retryCount = 3)
        {
            return await Policy.Handle<HttpRequestException>()
                               .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                               .ExecuteAsync(async () => await _httpClient.GetAsync(url, cancellationToken));
        }
    }
}

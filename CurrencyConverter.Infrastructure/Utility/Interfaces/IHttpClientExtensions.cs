namespace CurrencyConverter.Infrastructure.Utility.Interfaces
{
    public interface IHttpClientExtensions
    {
        Task<HttpResponseMessage> GetWithRetryAsync(string url, CancellationToken cancellationToken, int retryCount = 3);
    }
}

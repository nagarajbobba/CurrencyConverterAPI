using CurrencyConverter.Catching;

namespace CurrencyConverter.Infrastructure.Utility.Interfaces
{
    public interface ICacheHelper
    {
        Task SetAsync<T>(string key, T value, uint expirationInMinutes = 60, bool slidingExpirationEnabled = true) where T : class;
        Task<CacheItem<T>> GetAsync<T>(string key) where T : class;
    }
}

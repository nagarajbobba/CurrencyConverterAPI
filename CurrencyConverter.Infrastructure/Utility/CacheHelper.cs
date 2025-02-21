using CurrencyConverter.Catching;
using CurrencyConverter.Infrastructure.Utility.Interfaces;

namespace CurrencyConverter.Infrastructure.Utility
{
    public class CacheHelper : ICacheHelper
    {
        private readonly ICacheProvider _cacheProvider;
        public CacheHelper(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public async Task<CacheItem<T>> GetAsync<T>(string key) where T : class
        {
          return await _cacheProvider.GetAsync<T>(key);
        }

        public async Task SetAsync<T>(string key, T value, uint expirationInMinutes = 60, bool slidingExpirationEnabled = true) where T : class
        {
            var cacheItem = new CacheItem<T>
            {
                Key = key,
                Value = value,
                ExpirationInMinutes = expirationInMinutes,
                SlidingExpirationEnabled = slidingExpirationEnabled
            };

            await _cacheProvider.SetAsync(key, cacheItem);
        }
    }
}

namespace CurrencyConverter.Catching
{
    public interface ICacheProvider
    {
        Task<CacheItem<T>> GetAsync<T>(string key) where T : class;
        Task<bool> SetAsync<T>(string key, CacheItem<T> cacheItem) where T : class;
        Task<bool> RemoveAsync(string key);
        Task FlushAsync();
        Task<List<string>> GetAllKeysAsync();

    }
}

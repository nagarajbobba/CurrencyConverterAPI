using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace CurrencyConverter.Catching
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IConfiguration _configuration;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

        private IConnectionMultiplexer? _connectionMultiplexer;

        private IDatabaseAsync? _cache;

        private bool _disposed;

        public RedisCacheProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void Connect()
        {
            CheckDisposed();
            if (_cache != null)
            {
                return;
            }

            _connectionLock.Wait();
            try
            {
                if (_cache == null)
                {
                    _connectionMultiplexer = ConnectionMultiplexer.Connect(_configuration.GetSection("RedisCacheProvider")["ConnectionString"]);
                    _cache = _connectionMultiplexer.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
        private IServer GetServer()
        {
            return _connectionMultiplexer!.GetServer(_connectionMultiplexer.GetEndPoints()[0]);
        }
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _connectionMultiplexer?.Close();
            }
        }
        public async Task FlushAsync()
        {
            Connect();
            IAsyncEnumerable<RedisKey> asyncEnumerable = GetServer().KeysAsync(-1, "*", 250, 0L);
            await foreach (RedisKey item in asyncEnumerable)
            {
                await _cache!.KeyDeleteAsync(item);
            }
        }

        public async Task<List<string>> GetAllKeysAsync()
        {
            Connect();
            List<string> allKeys = new List<string>();
            IAsyncEnumerable<RedisKey> asyncEnumerable = GetServer().KeysAsync(-1, "*", 250, 0L);
            await foreach (RedisKey item in asyncEnumerable)
            {
                allKeys.Add(item);
            }

            return allKeys;
        }

        public async Task<CacheItem<T>> GetAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            Connect();
            RedisValue result = _cache!.StringGetAsync(new RedisKey(key)).Result;
            if (result.IsNullOrEmpty)
            {
                return null!;
            }

            CacheItem<T>? cacheValue = JsonSerializer.Deserialize<CacheItem<T>>(result!);
            if (cacheValue != null && cacheValue.SlidingExpirationEnabled)
            {
                await SetAsync(key, cacheValue);
            }

            return cacheValue!;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            Connect();
            return await _cache!.KeyDeleteAsync(new RedisKey(key));
        }

        public async Task<bool> SetAsync<T>(string key, CacheItem<T> cacheItem) where T : class
        {
            if (cacheItem == null)
            {
                throw new ArgumentNullException(nameof(cacheItem));
            }

            if (string.IsNullOrEmpty(cacheItem.Key))
            {
                throw new ArgumentNullException(nameof(cacheItem.Key));
            }

            if (cacheItem.Value == null)
            {
                throw new ArgumentNullException(nameof(cacheItem.Value));
            }

            string value = JsonSerializer.Serialize(cacheItem);
            Connect();
            return await _cache!.StringSetAsync(new RedisKey(cacheItem.Key), new RedisValue(value), TimeSpan.FromMinutes(cacheItem.ExpirationInMinutes));
        }
    }
}

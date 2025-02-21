namespace CurrencyConverter.Catching
{
    public class CacheItem<T> where T : class
    {
        public string Key { get; set; }

        public T Value { get; set; }

        public uint ExpirationInMinutes { get; set; }

        public bool SlidingExpirationEnabled { get; set; }

        public CacheItem()
        {
            ExpirationInMinutes = 1440u;
        }
    }
}

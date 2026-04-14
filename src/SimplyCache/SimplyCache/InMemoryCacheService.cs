namespace SimplyCache
{
    public interface IInMemoryCacheService
    {
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        T Get<T>(string key);
        bool TryGetValue<T>(string key, out T value);
        void Remove(string key);
        void Clear();
    }

    public class InMemoryCacheService : IInMemoryCacheService
    {
        private readonly Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();
        private readonly SimplyCacheOptions _options;

        public InMemoryCacheService(SimplyCacheOptions options)
        {
            _options = options;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var cacheItem))
            {
                if (cacheItem.Expiration < DateTime.UtcNow)
                {
                    _cache.Remove(key);
                    return default!;
                }
                else if (cacheItem.Type == typeof(T))
                {
                    return (T)cacheItem.Value;
                }
                else
                {
                    throw new InvalidCastException($"Cached value for key '{key}' is of type {cacheItem.Type}, not {typeof(T)}.");
                }
            }

            return default!;
        }

        public void Remove(string key)
        {
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
            }
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var effectiveExpiration = expiration ?? TimeSpan.FromSeconds(_options.DefaultCacheDurationInSeconds);
            _cache[key] = new CacheItem
            {
                Type = typeof(T),
                Value = value!,
                Expiration = DateTime.UtcNow.Add(effectiveExpiration)
            };
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (_cache.TryGetValue(key, out var cacheItem))
            {
                if (cacheItem.Expiration < DateTime.UtcNow)
                {
                    _cache.Remove(key);
                    value = default!;
                    return false;
                }
                else if (cacheItem.Type == typeof(T))
                {
                    value = (T)cacheItem.Value;
                    return true;
                }
                else
                {
                    value = default!;
                    return false;
                }
            }
            else
            {
                value = default!;
                return false;
            }
        }
    }
}

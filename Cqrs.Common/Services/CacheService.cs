using Microsoft.Extensions.Caching.Memory;
using System;
using Cqrs.Common.Interfaces;

namespace Cqrs.Common.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetOrAddValue<T>(string key, Func<T> factory, int expirationInSeconds = 0)
        {
            if (_cache.TryGetValue<T>(key, out var value))
            {
                return value;
            }

            value = factory();
            SetValue(key, value, expirationInSeconds);

            return value;
        }

        public T GetValue<T>(string key)
        {
            if (_cache.TryGetValue<T>(key, out var value))
            {
                return value;
            }

            return default;
        }

        public void SetValue<T>(string key, T value, int expirationInSeconds = 0)
        {
            var options = new MemoryCacheEntryOptions();

            if (expirationInSeconds != 0)
            {
                options.SetAbsoluteExpiration(TimeSpan.FromSeconds(expirationInSeconds));
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromDays(7));
            }

            _cache.Set(key, value, options);
        }
    }
}

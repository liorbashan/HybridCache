using HybridCache.Models.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace HybridCache.Services
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly int _defaultTTLInMinutes;

        public MemoryCacheService(IMemoryCache memoryCache, IOptions<MemoryCacheConfig> options)
        {
            _memoryCache = memoryCache;
            _defaultTTLInMinutes = options.Value.DefaultTTLInMinutes;
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(_defaultTTLInMinutes));
        }

        public T? Get<T>(string key)
        {
            return _memoryCache.TryGetValue(key, out T? value) ? value : default;
        }
    }
}

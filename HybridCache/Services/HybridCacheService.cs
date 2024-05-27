using HybridCache.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace HybridCache.Services
{
    public class HybridCacheService : IMultiLayerCache
    {
        private readonly MemoryCacheService _memoryCacheService;
        private readonly IDistributedCacheProvider? _distributedCacheService;
        private readonly bool _isDistributedCacheAvailable;

        public HybridCacheService(MemoryCacheService memoryCacheService, IDistributedCacheProvider? distributedCacheService = null)
        {
            _memoryCacheService = memoryCacheService;
            _distributedCacheService = distributedCacheService;
            _isDistributedCacheAvailable = _distributedCacheService != null;
        }
        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan memoryCacheDuration, TimeSpan? distributedCacheDuration = null)
        {
            try
            {
                var value = GetFromMemory<T>(key);
                if (value != null)
                    return value;

                value = await GetFromDistribution<T>(key);
                if (value != null)
                {
                    SetToMemory(key, value, memoryCacheDuration);
                    return value;
                }

                value = await factory();
                SetToMemory(key, value, memoryCacheDuration);
                var serializedData = JsonSerializer.SerializeToUtf8Bytes(value);
                await SetToDistribution(key, serializedData, distributedCacheDuration);

                return value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = GetFromMemory<T>(key);
                if (value != null)
                    return value;

                value = await GetFromDistribution<T>(key);
                if (value != null)
                {
                    SetToMemory(key, value);
                    return value;
                }
                
                return default;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan memoryCacheDuration, TimeSpan? distributedCacheDuration = null)
        {
            try
            {
                SetToMemory(key, value, memoryCacheDuration);
                //var serializedData = JsonSerializer.SerializeToUtf8Bytes(value);
                await SetToDistribution(key, value, distributedCacheDuration);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private T? GetFromMemory<T>(string key)
        {
            try
            {
                return _memoryCacheService.Get<T>(key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetToMemory<T>(string key, T value, TimeSpan? memoryCacheDuration = null)
        {
            try
            {
                _memoryCacheService.Set(key, value, memoryCacheDuration);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<T?> GetFromDistribution<T>(string key)
        {
            if (!_isDistributedCacheAvailable) return default;

            try
            {
                var cachedData = await _distributedCacheService!.GetAsync<string>(key);
                if (cachedData != null)
                {
                    var value = JsonSerializer.Deserialize<T>(cachedData);
                    return value;
                }
                return default(T?);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SetToDistribution(string key, object value, TimeSpan? distributedCacheDuration = null)
        {
            if (!_isDistributedCacheAvailable) return;

            try
            {
                await _distributedCacheService!.SetAsync(key, value, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = distributedCacheDuration
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

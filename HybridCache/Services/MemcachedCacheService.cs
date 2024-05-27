using Enyim.Caching.Memcached;
using HybridCache.Abstractions;
using HybridCache.Models.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HybridCache.Services
{
    public class MemcachedCacheService : IDistributedCacheProvider
    {
        private readonly int _defaultTTLInMinutes;
        private readonly IMemcachedClient _memcachedClient;

        public MemcachedCacheService(IMemcachedClient memcachedClient, IOptions<MemcachedOptions> options)
        {
            _memcachedClient = memcachedClient;
            _defaultTTLInMinutes = options.Value.DefaultTTLInMinutes;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _memcachedClient.GetAsync(key);
            return (T)value;
        }

        public async Task<bool> SetAsync(string key, object value, DistributedCacheEntryOptions options)
        {
            var expiration = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(_defaultTTLInMinutes);
            var result = await _memcachedClient.AddAsync(key, value, expiration);
            return result;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var result =  await _memcachedClient.DeleteWithResultAsync(key);
            return result.Success;
        }

        //public byte[] Get(string key)
        //{
        //    return _memcachedClient.GetAsync<byte[]>(key).Result;
        //}

        //public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        //{
        //    var value =  await _memcachedClient.GetAsync(key);
        //    return JsonSerializer.SerializeToUtf8Bytes(value);
        //}

        //public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        //{
        //    var expiration = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(_defaultTTLInMinutes);
        //    Task.Run(() => _memcachedClient.SetAsync(key, value, expiration));
        //}

        //public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        //{
        //    var expiration = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(_defaultTTLInMinutes);
        //    await _memcachedClient.SetAsync(key, value, expiration);
        //}


        //public void Refresh(string key)
        //{
        //    _memcachedClient.IncrementAsync(key);
        //}

        //public async Task RefreshAsync(string key, CancellationToken token = default)
        //{
        //    await _memcachedClient.IncrementAsync(key);
        //}

        //public void Remove(string key)
        //{
        //    _memcachedClient.DeleteAsync(key);
        //}

        //public async Task RemoveAsync(string key, CancellationToken token = default)
        //{
        //    await _memcachedClient.DeleteWithResultAsync(key);
        //}

        
    }
}

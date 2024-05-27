using System;
using System.Threading.Tasks;

namespace HybridCache.Abstractions
{
    public interface IMultiLayerCache
    {
        public Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan memoryCacheDuration, TimeSpan? distributedCacheDuration = null);
        public Task<T?> GetAsync<T>(string key);
        public Task SetAsync<T>(string key, T value, TimeSpan memoryCacheDuration, TimeSpan? distributedCacheDuration = null);
        
    }
}

using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace HybridCache.Abstractions
{
    public interface IDistributedCacheProvider
    {
        Task<T?> GetAsync<T>(string key);
        Task<bool> SetAsync(string key, object value, DistributedCacheEntryOptions options);
        Task<bool> DeleteAsync(string key);
    }
}

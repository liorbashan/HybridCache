using HybridCache.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridCacheTester
{
    public class App
    {
        private readonly IMultiLayerCache _cache;
        public App(IMultiLayerCache cache)
        {
            _cache = cache;
        }
        public async Task Run()
        {
            Console.WriteLine("app starting");
            // Example usage of the cache service
            string cacheKey = "sampleKey";
            string cacheValue = "sampleValue";

            // Add to cache
            await _cache.SetAsync(cacheKey, cacheValue, TimeSpan.FromMinutes(5));
            Console.WriteLine($"Set cache: {cacheKey} = {cacheValue}");

            // Retrieve from cache
            var retrievedValue = await _cache.GetAsync<string>(cacheKey);
            var retrievedValue2 = await _cache.GetAsync<string>("lior");
            Console.WriteLine($"Retrieved from cache: {cacheKey} = {retrievedValue}");
            Console.WriteLine($"Retrieved from cache: {"lior"} = {retrievedValue2}");

        }
    }
}

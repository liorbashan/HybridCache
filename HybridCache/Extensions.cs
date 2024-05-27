using Enyim.Caching.Memcached;
using HybridCache.Abstractions;
using HybridCache.Models.Configuration;
using HybridCache.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HybridCache
{
    public static class Extensions
    {
        public static IServiceCollection AddHybridCacheServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MemcachedOptions>(options => configuration.GetSection("MemcachedOptions").Bind(options));
            services.Configure<MemoryCacheConfig>(options => configuration.GetSection("MemoryCacheConfig").Bind(options));

            services.AddMemoryCache();

            var memcachedOptions = configuration.GetSection("MemcachedOptions").Get<MemcachedOptions>();
            var memcachedClient = CreateMemcachedClient(memcachedOptions);
            if (memcachedClient != null)
            {
                services.AddSingleton<IMemcachedClient>(memcachedClient);
                services.AddSingleton<IDistributedCacheProvider, MemcachedCacheService>();
            }
            else
            {
                services.AddSingleton<IMemcachedClient>(sp => null);
            }


            services.AddSingleton<MemoryCacheService>();
            services.AddSingleton<IMultiLayerCache, HybridCacheService>();

            return services;
        }

        private static IMemcachedClient? CreateMemcachedClient(MemcachedOptions options)
        {
            try
            {
                var cluster = new MemcachedCluster($"{options.Host}:{options.Port}");
                cluster.Start();
                var client = cluster.GetClient();

                return client;
            }
            catch
            {
                return null;
            }
        }
    }
}

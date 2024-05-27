
namespace HybridCache.Models.Configuration
{
    public class MemcachedOptions
    {
        public string? Host { get; set; }
        public string? Port { get; set; }
        public int DefaultTTLInMinutes { get; set; }
    }
}

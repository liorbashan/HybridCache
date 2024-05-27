using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HybridCache;
using HybridCacheTester;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", true, true)
    //.AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection()
    .AddHybridCacheServices(config)
    .AddTransient<App>();

var serviceProvider = serviceCollection.BuildServiceProvider();

await serviceProvider.GetService<App>().Run();

using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Polly.Caching
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMemoryCacheProvider(this IServiceCollection services)
        {
            return services.AddMemoryCache()
                           .AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();
        }

        public static IServiceCollection AddRedisCacheProvider(this IServiceCollection services, string configuration)
        {
            return services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration))
                           .AddSingleton<IAsyncCacheProvider, RedisCacheProvider>();
        }
    }
}
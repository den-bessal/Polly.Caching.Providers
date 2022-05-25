using Microsoft.Extensions.DependencyInjection;

namespace Polly.Caching
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMemoryCacheProvider(this IServiceCollection services)
        {
            return services.AddMemoryCache()
                           .AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();
        }
    }
}
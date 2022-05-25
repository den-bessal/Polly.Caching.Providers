using Microsoft.Extensions.Caching.Memory;

namespace Polly.Caching
{
    public class MemoryCacheProvider : ISyncCacheProvider, IAsyncCacheProvider
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public (bool, object) TryGet(string key)
        {
            bool cacheHit = _cache.TryGetValue(key, out var value);
            return (cacheHit, value);
        }

        public void Put(string key, object value, Ttl ttl)
        {
            var remaining = DateTimeOffset.MaxValue - DateTimeOffset.UtcNow;
            var options = new MemoryCacheEntryOptions();

            if (ttl.SlidingExpiration)
            {
                options.SlidingExpiration = ttl.Timespan < remaining ? ttl.Timespan : remaining;
            }
            else
            {
                if (ttl.Timespan == TimeSpan.MaxValue)
                {
                    options.AbsoluteExpiration = DateTimeOffset.MaxValue;
                }
                else
                {
                    options.AbsoluteExpirationRelativeToNow = ttl.Timespan < remaining ? ttl.Timespan : remaining;
                }
            }

            _cache.Set(key, value, options);
        }

        public Task<(bool, object)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(TryGet(key));
        }

        public Task PutAsync(string key, object value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Put(key, value, ttl);
            return Task.CompletedTask;
        }
    }
}
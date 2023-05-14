using Newtonsoft.Json;
using StackExchange.Redis;

namespace Polly.Caching
{
    public class RedisCacheProvider : ISyncCacheProvider, IAsyncCacheProvider
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheProvider(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public (bool, object) TryGet(string key)
        {
            var value = default(object);
            var json = _redis.GetDatabase().StringGet(key);
            
            if (json.HasValue)
            {
                var serializationSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                value = JsonConvert.DeserializeObject(json, serializationSettings);
            }

            return (json.HasValue, value);
        }

        public void Put(string key, object value, Ttl ttl)
        {
            var serializationSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var json = JsonConvert.SerializeObject(value, serializationSettings);
            _redis.GetDatabase().StringSet(key, json, ttl.Timespan);
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
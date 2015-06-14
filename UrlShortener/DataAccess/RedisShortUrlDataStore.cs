using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace UrlShortener.DataAccess
{
    public class RedisShortUrlDataStore : IShortUrlDataStore
    {
        private readonly Lazy<ConnectionMultiplexer> redis;
        private readonly string shortUrlIdKey;

        public RedisShortUrlDataStore()
        {
            redis = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("localhost")); //TODO use connection string
            shortUrlIdKey = String.Format("shortUrlId[{0}]", "example.com"); // TODO use short url base from config
        }

        public async Task<bool> TryAdd(string urlKey, string url)
        {
            return await GetDatabase().StringSetAsync(urlKey, url, when: When.NotExists);
        }

        public async Task<string> GetUrl(string urlKey)
        {
            return await GetDatabase().StringGetAsync(urlKey);
        }

        public async Task<long> GetNextShortUrlId()
        {
            return await GetDatabase().StringIncrementAsync(shortUrlIdKey);
        }

        private IDatabase GetDatabase()
        {
            return redis.Value.GetDatabase();
        }
    }
}

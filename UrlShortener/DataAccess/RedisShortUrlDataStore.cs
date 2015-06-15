using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using UrlShortener.Configuration;

namespace UrlShortener.DataAccess
{
    public class RedisShortUrlDataStore : IShortUrlDataStore
    {
        private readonly Lazy<ConnectionMultiplexer> redis;
        private readonly string shortUrlIdKey;

        public RedisShortUrlDataStore(IConfigurationProvider configurationProvider)
        {
            redis = new Lazy<ConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(configurationProvider.RedisConnectionString));
            shortUrlIdKey = String.Format("shortUrlId[{0}]", configurationProvider.ShortUrlBase);
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

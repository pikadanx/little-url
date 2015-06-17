using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using UrlShortener.Configuration;
using UrlShortener.Exceptions;

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
            try
            {
                return await GetDatabase().StringSetAsync(urlKey, url, when: When.NotExists);
            }
            catch (RedisConnectionException e)
            {
                throw new ServiceUnavailableException("Not able to connect to Redis", e);
            }
            
        }

        public async Task<string> GetUrl(string urlKey)
        {
            try
            {
                return await GetDatabase().StringGetAsync(urlKey);
            }
            catch (RedisConnectionException e)
            {
                throw new ServiceUnavailableException("Not able to connect to Redis", e);
            }
        }

        public async Task<long> GetNextShortUrlId()
        {
            try
            {
                return await GetDatabase().StringIncrementAsync(shortUrlIdKey);
            }
            catch (RedisConnectionException e)
            {
                throw new ServiceUnavailableException("Not able to connect to Redis", e);
            }
        }

        private IDatabase GetDatabase()
        {
            return redis.Value.GetDatabase();
        }
    }
}

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
        private const string ShortUrlIdKey = "[shortUrlId]";

        public RedisShortUrlDataStore(IConfigurationProvider configurationProvider)
        {
            redis = new Lazy<ConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(configurationProvider.RedisConnectionString));
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
                return await GetDatabase().StringIncrementAsync(ShortUrlIdKey);
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

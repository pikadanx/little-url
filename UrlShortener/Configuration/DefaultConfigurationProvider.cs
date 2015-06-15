using System;
using System.Configuration;

namespace UrlShortener.Configuration
{
    public class DefaultConfigurationProvider : IConfigurationProvider
    {
        private readonly Lazy<string> shortUrlBase =
            new Lazy<string>(() => ConfigurationManager.AppSettings["UrlShortener.ShortUrlBase"]);
        private readonly Lazy<string> redisConnectionString =
            new Lazy<string>(() => ConfigurationManager.ConnectionStrings["UrlShortener.Redis"].ConnectionString);

        public string ShortUrlBase
        {
            get { return shortUrlBase.Value; }
        }

        public string RedisConnectionString
        {
            get { return redisConnectionString.Value; }
        }
    }
}

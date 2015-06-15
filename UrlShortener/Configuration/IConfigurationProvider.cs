
namespace UrlShortener.Configuration
{
    public interface IConfigurationProvider
    {
        string ShortUrlBase { get; }

        string RedisConnectionString { get; }
    }
}

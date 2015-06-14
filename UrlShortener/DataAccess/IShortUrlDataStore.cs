
namespace UrlShortener.DataAccess
{
    public interface IShortUrlDataStore
    {
        bool TryAdd(string urlKey, string url);

        bool TryGetUrl(string urlKey, out string url);

        long GetNextShortUrlId();
    }
}

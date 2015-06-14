
namespace UrlShortener
{
    public interface IShortUrlResolver
    {
        bool TryGetUrl(string urlKey, out string url);
    }
}

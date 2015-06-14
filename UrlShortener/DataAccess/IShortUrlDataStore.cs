using System.Threading.Tasks;

namespace UrlShortener.DataAccess
{
    public interface IShortUrlDataStore
    {
        Task<bool> TryAdd(string urlKey, string url);

        Task<string> GetUrl(string urlKey);

        Task<long> GetNextShortUrlId();
    }
}

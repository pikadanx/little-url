using System.Threading.Tasks;

namespace UrlShortener
{
    public interface IShortUrlResolver
    {
        Task<string> GetUrl(string urlKey);
    }
}

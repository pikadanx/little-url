using System.Threading.Tasks;

namespace UrlShortener
{
    public interface IShortUrlGenerator
    {
        Task<string> GetNextShortUrlHash();
    }
}

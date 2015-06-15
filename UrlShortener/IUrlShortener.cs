using System.Threading.Tasks;

namespace UrlShortener
{
    public interface IUrlShortener
    {
        Task<string> CreateShortUrl(string url);
    }
}
using System.Threading.Tasks;
using UrlShortener.DataAccess;

namespace UrlShortener
{
    public class ShortUrlResolver : IShortUrlResolver
    {
        private readonly IShortUrlDataStore dataStore;

        public ShortUrlResolver(IShortUrlDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public async Task<string> GetUrl(string urlKey)
        {
            return await dataStore.GetUrl(urlKey);
        }
    }
}

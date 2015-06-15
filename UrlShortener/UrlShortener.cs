using System;
using System.Threading.Tasks;
using UrlShortener.DataAccess;

namespace UrlShortener
{
    public class UrlShortener
    {
        private readonly IShortUrlDataStore dataStore;
        private readonly IShortUrlGenerator shortUrlGenerator;
        private const int MaxTryCount = 3;

        public UrlShortener(IShortUrlDataStore dataStore, IShortUrlGenerator shortUrlGenerator)
        {
            this.dataStore = dataStore;
            this.shortUrlGenerator = shortUrlGenerator;
        }

        public async Task<string> CreateShortUrl(string url)
        {
            for (var tryCount = 0; tryCount < MaxTryCount; tryCount++)
            {
                var shortUrlHash = await shortUrlGenerator.GetNextShortUrlHash();
                if (await dataStore.TryAdd(shortUrlHash, url))
                {
                    return shortUrlHash;
                }
            }

            return String.Empty;
        }
    }
}

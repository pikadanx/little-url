using System;
using System.Threading.Tasks;
using UrlShortener.Configuration;
using UrlShortener.DataAccess;

namespace UrlShortener
{
    public class UrlShortener : IUrlShortener
    {
        private readonly IShortUrlDataStore dataStore;
        private readonly IShortUrlGenerator shortUrlGenerator;
        private readonly IConfigurationProvider configurationProvider;
        private const int MaxTryCount = 3;

        public UrlShortener(IShortUrlDataStore dataStore, IShortUrlGenerator shortUrlGenerator,
            IConfigurationProvider configurationProvider)
        {
            this.dataStore = dataStore;
            this.shortUrlGenerator = shortUrlGenerator;
            this.configurationProvider = configurationProvider;
        }

        public async Task<string> CreateShortUrl(string url)
        {
            for (var tryCount = 0; tryCount < MaxTryCount; tryCount++)
            {
                var shortUrlHash = await shortUrlGenerator.GetNextShortUrlHash();
                if (await dataStore.TryAdd(shortUrlHash, url))
                {
                    return configurationProvider.ShortUrlBase + shortUrlHash;
                }
            }

            return String.Empty;
        }
    }
}

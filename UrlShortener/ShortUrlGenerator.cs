using System.Text;
using System.Threading.Tasks;
using UrlShortener.DataAccess;

namespace UrlShortener
{
    public class ShortUrlGenerator : IShortUrlGenerator
    {
        private readonly IShortUrlDataStore dataStore;
        private const string UriUnreservedCharacters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-._~"; // from https://tools.ietf.org/html/rfc3986#section-2.3
        private static readonly int Base = UriUnreservedCharacters.Length - 1; // reserve last char for negative numbers

        public ShortUrlGenerator(IShortUrlDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public async Task<string> GetNextShortUrlHash()
        {
            var id = await dataStore.GetNextShortUrlId();

            return EncodeId(id);
        }

        private static string EncodeId(long id)
        {
            if (id == 0)
            {
                return UriUnreservedCharacters[0].ToString();
            }

            bool negative = id < 0;
            var sb = new StringBuilder();

            if (negative)
            {
                sb.Append(UriUnreservedCharacters[Base]);
                id *= -1;
            }

            while (id > 0)
            {
                sb.Insert(negative ? 1 : 0, UriUnreservedCharacters[(int) (id%Base)]);
                id = id / Base;
            }

            return sb.ToString();
        }
    }
}

using System;

namespace UrlShortener
{
    public static class UrlHelpers
    {
        public static bool TryGetUrlKeyFromShortUrl(string shortUrl, out string urlKey)
        {
            shortUrl = GetAsCanonialHttpUrl(shortUrl);

            Uri url;
            if (Uri.TryCreate(shortUrl, UriKind.Absolute, out url))
            {
                urlKey = url.AbsolutePath.Split('/')[1];
                return !String.IsNullOrEmpty(urlKey);
            }

            urlKey = String.Empty;
            return false;
        }

        public static string GetAsCanonialHttpUrl(string url)
        {
            if (String.IsNullOrWhiteSpace(url) || url.Contains("://"))
            {
                return url;
            }

            if (url.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            {
                url = "http:" + url;
            }
            else if (!(url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                       url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)))
            {
                url = "http://" + url;
            }
            return url;
        }
    }
}

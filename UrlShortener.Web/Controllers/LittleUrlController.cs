using System.Web.Http;

namespace UrlShortener.Web.Controllers
{
    public class LittleUrlController : ApiController
    {
        private readonly IUrlShortener urlShortener;
        private readonly IShortUrlResolver shortUrlResolver;

        public LittleUrlController(IUrlShortener urlShortener, IShortUrlResolver shortUrlResolver)
        {
            this.urlShortener = urlShortener;
            this.shortUrlResolver = shortUrlResolver;
        }
    }
}

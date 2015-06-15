using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using UrlShortener.Web.Models;

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

        [Route("{urlKey:regex(^([0-9]|[a-z]|[A-Z]|[-._~])+$)}")]
        public async Task<HttpResponseMessage> Get(string urlKey, bool redirect = true)
        {
            var url = await shortUrlResolver.GetUrl(urlKey);

            if (String.IsNullOrEmpty(url))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new {Error = "Short Url Not Found."});
            }

            if (redirect)
            {
                var response = Request.CreateResponse(HttpStatusCode.MovedPermanently);
                response.Headers.Location = new Uri(url);
                return response;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new {Url = url});
        }

        public async Task<HttpResponseMessage> Post([FromBody]CreateLittleUrlRequestContext context)
        {
            var url = context.Url; // TODO default scheme to Http if not present

            if (IsValidUrl(url))
            {
                var shortUrl = await urlShortener.CreateShortUrl(url);

                return Request.CreateResponse(HttpStatusCode.Created, new {LittleUrl = shortUrl});
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, new {Error = "Url is not valid."});
        }

        private static bool IsValidUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps); // TODO consider other Uri Schemes
        }
    }
}

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using UrlShortener.Exceptions;
using UrlShortener.Web.Models;

namespace UrlShortener.Web.Controllers
{
    [RoutePrefix("api")]
    public class LittleUrlController : ApiController
    {
        private readonly IUrlShortener urlShortener;
        private readonly IShortUrlResolver shortUrlResolver;

        public LittleUrlController(IUrlShortener urlShortener, IShortUrlResolver shortUrlResolver)
        {
            this.urlShortener = urlShortener;
            this.shortUrlResolver = shortUrlResolver;
        }

        [HttpGet]
        [Route("~/{urlKey:regex(^([0-9]|[a-z]|[A-Z]|[-._~])+$)}")]
        public async Task<HttpResponseMessage> RedirectLittleUrl(string urlKey)
        {
            try
            {
                var url = await shortUrlResolver.GetUrl(urlKey);

                if (String.IsNullOrEmpty(url))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new {Error = "Short Url Not Found."});
                }

                var response = Request.CreateResponse(HttpStatusCode.MovedPermanently);
                response.Headers.Location = new Uri(url);
                return response;
            }
            catch (ServiceUnavailableException)
            {
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
            }
        }

        [Route("preview")]
        public async Task<HttpResponseMessage> Get(string littleUrl)
        {
            try
            {
                string urlKey;
                if (!UrlHelpers.TryGetUrlKeyFromShortUrl(littleUrl, out urlKey))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var url = await shortUrlResolver.GetUrl(urlKey);

                if (String.IsNullOrEmpty(url))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { Error = "Short Url Not Found." });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { Url = url });
            }
            catch (ServiceUnavailableException)
            {
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
            }
        }

        [Route("create")]
        public async Task<HttpResponseMessage> Post([FromBody]CreateLittleUrlRequestContext context)
        {
            try
            {
                var url = UrlHelpers.GetAsCanonialHttpUrl(context.Url);

                if (IsValidUrl(url))
                {
                    var shortUrl = await urlShortener.CreateShortUrl(url);

                    return Request.CreateResponse(HttpStatusCode.Created, new {LittleUrl = shortUrl});
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, new {Error = "Url is not valid."});
            }
            catch (ServiceUnavailableException)
            {
                return Request.CreateResponse(HttpStatusCode.ServiceUnavailable);
            }
        }

        private static bool IsValidUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps); // TODO consider other Uri Schemes
        }
    }
}

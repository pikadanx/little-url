﻿using System;
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

        /// <summary>
        /// Redirects little url to it's associated url.
        /// </summary>
        /// <param name="urlKey"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the associated url for the given little url.
        /// </summary>
        /// <param name="littleUrl"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a little url for the given url.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [Route("create")]
        public async Task<HttpResponseMessage> Post([FromBody]CreateLittleUrlRequestContext context)
        {
            try
            {
                var url = UrlHelpers.GetAsCanonialHttpUrl(context.Url);

                if (IsValidUrl(url))
                {
                    var shortUrl = await urlShortener.CreateShortUrl(url);

                    return String.IsNullOrEmpty(shortUrl)
                        ? Request.CreateResponse(HttpStatusCode.InternalServerError,
                            new {Error = "Could not create little url."})
                        : Request.CreateResponse(HttpStatusCode.Created, new {LittleUrl = shortUrl});
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

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.Exceptions;
using UrlShortener.Web.Controllers;
using UrlShortener.Web.Models;

namespace UrlShortener.Web.Tests.Controllers
{
    [TestClass]
    public class LittleUrlControllerTests
    {
        private Mock<IUrlShortener> mockUrlShortener;
        private Mock<IShortUrlResolver> mockShortUrlResolver;
        private LittleUrlController littleUrlController;

        [TestInitialize]
        public void Init()
        {
            mockUrlShortener = new Mock<IUrlShortener>();
            mockShortUrlResolver = new Mock<IShortUrlResolver>();
            littleUrlController = new LittleUrlController(mockUrlShortener.Object, mockShortUrlResolver.Object)
            {
                Request = new HttpRequestMessage()
            };
            littleUrlController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }

        [TestMethod]
        public async Task RedirectLittleUrl_Returns404_WhenShortUrlNotFound()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>())).ReturnsAsync(String.Empty);

            var response = await littleUrlController.RedirectLittleUrl("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task RedirectLittleUrl_Returns301()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>())).ReturnsAsync("http://example.com");

            var response = await littleUrlController.RedirectLittleUrl("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.MovedPermanently);
            Assert.AreEqual(new Uri("http://example.com"), response.Headers.Location);
        }

        [TestMethod]
        public async Task RedirectLittleUrl_Returns503_WhenShortUrlResolverThrowsServiveUnavailable()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>()))
                .ThrowsAsync(new ServiceUnavailableException());

            var response = await littleUrlController.RedirectLittleUrl("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.ServiceUnavailable);
        }

        [TestMethod]
        public async Task Get_Returns404_WhenShortUrlNotFound()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>())).ReturnsAsync(String.Empty);

            var response = await littleUrlController.Get("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Get_Returns200()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>())).ReturnsAsync("http://example.com");

            var response = await littleUrlController.Get("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task Get_Returns503_WhenShortUrlResolverThrowsServiveUnavailable()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>()))
                .ThrowsAsync(new ServiceUnavailableException());

            var response = await littleUrlController.Get("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.ServiceUnavailable);
        }

        [TestMethod]
        public async Task Post_Returns400_WhenUrlIsInvalid()
        {
            var context = new CreateLittleUrlRequestContext
            {
                Url = "?bad:Url"
            };

            var response = await littleUrlController.Post(context);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Post_Returns201()
        {
            var context = new CreateLittleUrlRequestContext
            {
                Url = "http://example.com"
            };

            var response = await littleUrlController.Post(context);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task Post_Returns503_WhenUrlShortenerThrowsServiveUnavailable()
        {
            mockUrlShortener.Setup(u => u.CreateShortUrl(It.IsAny<string>()))
                .ThrowsAsync(new ServiceUnavailableException());

            var context = new CreateLittleUrlRequestContext
            {
                Url = "http://example.com"
            };

            var response = await littleUrlController.Post(context);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.ServiceUnavailable);
        }
    }
}

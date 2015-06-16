﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        public async Task Get_Returns404_WhenShortUrlNotFound()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>())).ReturnsAsync(String.Empty);

            var response = await littleUrlController.Get("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Get_Returns301_WhenRedirectIsTrue()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>())).ReturnsAsync("http://example.com");

            var response = await littleUrlController.Get("foo");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.MovedPermanently);
            Assert.AreEqual(new Uri("http://example.com"), response.Headers.Location);
        }

        [TestMethod]
        public async Task Get_Returns200_WhenRedirectIsFalse()
        {
            mockShortUrlResolver.Setup(r => r.GetUrl(It.IsAny<string>())).ReturnsAsync("http://example.com");

            var response = await littleUrlController.Get("foo", redirect: false);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
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
    }
}

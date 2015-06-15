using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.DataAccess;

namespace UrlShortener.Tests
{
    [TestClass]
    public class UrlShortenerTests
    {
        private Mock<IShortUrlGenerator> mockShortUrlGenerator;
        private Mock<IShortUrlDataStore> mockDataStore;
        private UrlShortener urlShortener;

        [TestInitialize]
        public void Init()
        {
            mockDataStore = new Mock<IShortUrlDataStore>();
            mockShortUrlGenerator = new Mock<IShortUrlGenerator>();
            urlShortener = new UrlShortener(mockDataStore.Object, mockShortUrlGenerator.Object);
        }

        [TestMethod]
        public async Task CreateShortUrl_CallsGetNextShortUrlHashOnShortUrlGenerator()
        {
            mockShortUrlGenerator.Setup(gen => gen.GetNextShortUrlHash())
                .ReturnsAsync("foo")
                .Verifiable("Expected IShortUrlGenerator.GetNextShortUrlHash() to be called.");

            await urlShortener.CreateShortUrl("http://example.com");

            mockDataStore.Verify();
        }

        [TestMethod]
        public async Task CreateShortUrl_CallsTryAddOnDataStore()
        {
            mockDataStore.Setup(store => store.TryAdd(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable("Expected IShortUrlDataStore.TryAdd() to be called.");

            await urlShortener.CreateShortUrl("http://example.com");

            mockDataStore.Verify();
        }

        [TestMethod]
        public async Task CreateShortUrl_CallsTryAddWithGivenUrlOnDataStore()
        {
            const string expectedUrl = "http://example.com";
            mockDataStore.Setup(store => store.TryAdd(It.IsAny<string>(), expectedUrl))
                .ReturnsAsync(true)
                .Verifiable(string.Format("Expected IShortUrlDataStore.TryAdd() to be called with url:'{0}'.", expectedUrl));

            await urlShortener.CreateShortUrl(expectedUrl);

            mockDataStore.Verify();
        }

        [TestMethod]
        public async Task CreateShortUrl_CallsTryAddWithHashFromShortUrlGeneratorOnDataStore()
        {
            const string expectedHash = "foo";
            mockShortUrlGenerator.Setup(gen => gen.GetNextShortUrlHash())
                .ReturnsAsync(expectedHash);
            mockDataStore.Setup(store => store.TryAdd(expectedHash, It.IsAny<string>()))
                .ReturnsAsync(true)
                .Verifiable(string.Format("Expected IShortUrlDataStore.TryAdd() to be called with urlKey:'{0}'.", expectedHash));

            await urlShortener.CreateShortUrl("http://example.com");

            mockDataStore.Verify();
        }

        [TestMethod]
        public async Task CreateShortUrl_ReturnsValueFromShortUrlGenerator_WhenAddedToDataStore()
        {
            const string expectedHash = "foo";
            mockShortUrlGenerator.Setup(gen => gen.GetNextShortUrlHash())
                .ReturnsAsync(expectedHash);
            mockDataStore.Setup(store => store.TryAdd(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var actualUrl = await urlShortener.CreateShortUrl("http://example.com");

            Assert.AreEqual(expectedHash, actualUrl);
        }

        [TestMethod]
        public async Task CreateShortUrl_ReturnsEmptyString_WhenUnableToAddToDataStore()
        {
            mockShortUrlGenerator.Setup(gen => gen.GetNextShortUrlHash())
                .ReturnsAsync("foo");
            mockDataStore.Setup(store => store.TryAdd(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var actualUrl = await urlShortener.CreateShortUrl("http://example.com");

            Assert.AreEqual(String.Empty, actualUrl);
        }

        [TestMethod]
        public async Task CreateShortUrl_ReturnsValueFromShortUrlGenerator_WhenAddedToDataStoreOnThirdTry()
        {
            const string expectedHash = "foo";
            var tryAddResults = new Queue<bool>(new[] {false, false, true});
            mockShortUrlGenerator.Setup(gen => gen.GetNextShortUrlHash())
                .ReturnsAsync(expectedHash);
            mockDataStore.Setup(store => store.TryAdd(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(tryAddResults.Dequeue()));

            var actualUrl = await urlShortener.CreateShortUrl("http://example.com");

            Assert.AreEqual(expectedHash, actualUrl);
            mockShortUrlGenerator.Verify(gen => gen.GetNextShortUrlHash(), Times.Exactly(3));
            mockDataStore.Verify(ds => ds.TryAdd(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));
        }
    }
}

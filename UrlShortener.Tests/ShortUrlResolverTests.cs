using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.DataAccess;

namespace UrlShortener.Tests
{
    [TestClass]
    public class ShortUrlResolverTests
    {
        private ShortUrlResolver shortUrlResolver;
        private Mock<IShortUrlDataStore> mockDataStore;

        [TestInitialize]
        public void Init()
        {
            mockDataStore = new Mock<IShortUrlDataStore>();
            shortUrlResolver = new ShortUrlResolver(mockDataStore.Object);
        }

        [TestMethod]
        public async Task GetUrl_CallsGetUrlOnDataStore()
        {
            mockDataStore.Setup(store => store.GetUrl(It.IsAny<string>()))
                .ReturnsAsync("http://example.com")
                .Verifiable("Expected IShortUrlDataStore.GetUrl() to be called.");

            await shortUrlResolver.GetUrl("foo");

            mockDataStore.Verify();
        }

        [TestMethod]
        public async Task GetUrl_CallsGetUrlWithGivenUrlKeyOnDataStore()
        {
            const string urlKey = "foo";
            mockDataStore.Setup(store => store.GetUrl(urlKey))
                .ReturnsAsync("http://example.com")
                .Verifiable(string.Format("Expected IShortUrlDataStore.GetUrl() to be called with '{0}'.", urlKey));

            await shortUrlResolver.GetUrl("foo");

            mockDataStore.Verify();
        }

        [TestMethod]
        public async Task GetUrl_ReturnsValueFromDataStore()
        {
            const string expectedUrl = "http://example.com";
            mockDataStore.Setup(store => store.GetUrl(It.IsAny<string>()))
                .ReturnsAsync(expectedUrl);

            var actualUrl = await shortUrlResolver.GetUrl("foo");

            Assert.AreEqual(expectedUrl, actualUrl);
        }
    }
}

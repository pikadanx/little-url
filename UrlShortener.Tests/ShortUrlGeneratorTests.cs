using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.DataAccess;

namespace UrlShortener.Tests
{
    [TestClass]
    public class ShortUrlGeneratorTests
    {
        private ShortUrlGenerator shortUrlGenerator;
        private Mock<IShortUrlDataStore> mockDataStore;

        [TestInitialize]
        public void Init()
        {
            mockDataStore = new Mock<IShortUrlDataStore>();
            shortUrlGenerator = new ShortUrlGenerator(mockDataStore.Object);
        }

        [TestMethod]
        public async Task GetNextShortUrlHash_CallsGetNextShortUrlIdOnDataStore()
        {
            mockDataStore.Setup(store => store.GetNextShortUrlId()).ReturnsAsync(0).Verifiable("Expected IShortUrlDataStore.GetNextShortUrlId() to be called.");

            await shortUrlGenerator.GetNextShortUrlHash();

            mockDataStore.Verify();
        }

        [TestMethod]
        public async Task GetNextShortUrlHash_Returns0_WhenIdIs0()
        {
            mockDataStore.Setup(store => store.GetNextShortUrlId()).ReturnsAsync(0);

            var hash = await shortUrlGenerator.GetNextShortUrlHash();

            Assert.AreEqual("0", hash);
        }

        [TestMethod]
        public async Task GetNextShortUrlHash_ReturnsTwoCharacters_WhenIdIsOneOverCharacterSetCount()
        {
            mockDataStore.Setup(store => store.GetNextShortUrlId()).ReturnsAsync(65);

            var hash = await shortUrlGenerator.GetNextShortUrlHash();

            Assert.AreEqual("10", hash);
        }

        [TestMethod]
        public async Task GetNextShortUrlHash_ReturnsValue_WhenIdIsMaxValue()
        {
            mockDataStore.Setup(store => store.GetNextShortUrlId()).ReturnsAsync(long.MaxValue);

            var hash = await shortUrlGenerator.GetNextShortUrlHash();

            Assert.AreEqual("6TkEooEkxO7", hash);
        }

        [TestMethod]
        public async Task GetNextShortUrlHash_ReturnsWithLeadingTilde_WhenIdIsNegativeValue()
        {
            mockDataStore.Setup(store => store.GetNextShortUrlId()).ReturnsAsync(-1);

            var hash = await shortUrlGenerator.GetNextShortUrlHash();

            Assert.AreEqual("~1", hash);
        }

        [TestMethod]
        public async Task GetNextShortUrlHash_ReturnsTilde_WhenIdIsMinValue()
        {
            mockDataStore.Setup(store => store.GetNextShortUrlId()).ReturnsAsync(long.MinValue);

            var hash = await shortUrlGenerator.GetNextShortUrlHash();

            Assert.AreEqual("~", hash);
        }
    }
}

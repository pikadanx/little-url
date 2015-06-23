using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UrlShortener.Tests
{
    [TestClass]
    public class UrlHelpersTests
    {
        [TestMethod]
        public void TryGetUrlKeyFromShortUrl_ReturnsTrueAndKey()
        {
            string result;
            Assert.IsTrue(UrlHelpers.TryGetUrlKeyFromShortUrl("http://example.com/foo", out result));

            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void TryGetUrlKeyFromShortUrl_ReturnsTrueAndKey_WhenSlashIsAfterKey()
        {
            string result;
            Assert.IsTrue(UrlHelpers.TryGetUrlKeyFromShortUrl("http://example.com/foo/", out result));

            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void TryGetUrlKeyFromShortUrl_ReturnsTrueAndKey_WhenQuestingMarkIsAfterKey()
        {
            string result;
            Assert.IsTrue(UrlHelpers.TryGetUrlKeyFromShortUrl("http://example.com/foo?", out result));

            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void TryGetUrlKeyFromShortUrl_ReturnsTrueAndKey_WhenSchemeIsMissing()
        {
            string result;
            Assert.IsTrue(UrlHelpers.TryGetUrlKeyFromShortUrl("example.com/foo?", out result));

            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void TryGetUrlKeyFromShortUrl_ReturnsTrueAndKey_WhenBeginsWithSlashSlash()
        {
            string result;
            Assert.IsTrue(UrlHelpers.TryGetUrlKeyFromShortUrl("//example.com/foo?", out result));

            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void TryGetUrlKeyFromShortUrl_ReturnsFalse_WhenKeyNotInPath()
        {
            string result;
            Assert.IsFalse(UrlHelpers.TryGetUrlKeyFromShortUrl("http://example.com", out result));
        }

        [TestMethod]
        public void TryGetUrlKeyFromShortUrl_ReturnsFalse_WhenKeyNotInPathWithQuery()
        {
            string result;
            Assert.IsFalse(UrlHelpers.TryGetUrlKeyFromShortUrl("http://example.com?key=value", out result));
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlAsIs_WhenUrlHasHttpScheme()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl("http://example.com");

            Assert.AreEqual("http://example.com", result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlAsIs_WhenUrlHasHttpsScheme()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl("https://example.com");

            Assert.AreEqual("https://example.com", result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlWithHttpScheme_WhenUrlHasNoScheme()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl("example.com");

            Assert.AreEqual("http://example.com", result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlWithHttpScheme_WhenUrlHasNoSchemeAndStartsWithSlashSlash()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl("//example.com");

            Assert.AreEqual("http://example.com", result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlWithHttpScheme_WhenUrlHasNoSchemeAndStartsWithHttp()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl("httpexample.com");

            Assert.AreEqual("http://httpexample.com", result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlAsIs_WhenUrlIsNull()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlAsIs_WhenUrlIsEmptyString()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl(String.Empty);

            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlAsIs_WhenUrlIsWhiteSpace()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl("   ");

            Assert.AreEqual("   ", result);
        }

        [TestMethod]
        public void GetAsCanonialHttpUrl_ReturnsUrlAsIs_WhenUrlSchemeIsFtp()
        {
            var result = UrlHelpers.GetAsCanonialHttpUrl("ftp://example.com");

            Assert.AreEqual("ftp://example.com", result);
        }
    }
}

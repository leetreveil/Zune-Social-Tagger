using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.ZuneWebsiteScraper
{
    [TestFixture]
    public class PageDownloaderTests : AsyncTesting
    {
        private const string Webpage = "http://www.google.co.uk/";

        [Test]
        public void Should_be_able_to_download_a_webpage_from_a_url_to_a_string()
        {
            string result = PageDownloader.Download(Webpage);

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Should_be_able_to_download_a_webpage_from_a_url_asyncronously()
        {
            PageDownloader.DownloadAsync(Webpage, page =>
                                                      {
                                                          Assert.That(page.Length, Is.GreaterThan(0));
                                                          base.Set();
                                                      });

            base.WaitOneWith500MsTimeoutAnd("did not download webpage");

            Assert.That(Assert.Counter, Is.EqualTo(1));
        }
    }
}
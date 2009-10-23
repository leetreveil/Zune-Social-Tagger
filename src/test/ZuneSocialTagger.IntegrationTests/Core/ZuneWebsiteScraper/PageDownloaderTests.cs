using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenAValidUrlIsProvided : AsyncTesting
    {
        private const string Webpage = "http://www.google.co.uk/";

        [Test]
        public void Then_it_should_be_able_to_download_the_webpage_as_a_string_syncronously()
        {
            string result = PageDownloader.Download(Webpage);

            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Then_it_should_be_able_to_download_the_webpage_as_a_string_asyncronously()
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

    [TestFixture]
    public class WhenAnInvalidUrlIsProvided
    {
        [Test]
        [ExpectedException(typeof(PageDownloaderException))]
        public void Then_it_should_throw_an_PageDownloaderException()
        {
            PageDownloader.Download("htzp://www.asdasda.com");

        }

        [TestFixture]
        public class WhenAValidUrlIsProvidedButThereIsNoResponseFromTheWebsite
        {
            [Test]
            [ExpectedException(typeof(PageDownloaderException))]
            public void Then_it_should_throw_an_PageDownloaderException()
            {
                PageDownloader.Download("http://www.hasdhashdahsdhasdwqdygygqwefgywe.com");
            }
        }
    }
}
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsite
{
    [TestFixture]
    public class WhenAValidUrlIsProvided
    {
        private const string Webpage = "http://www.google.co.uk/";

        [Test]
        public void Then_it_should_be_able_to_download_the_webpage_as_a_string_syncronously()
        {
            string result = PageDownloader.Download(Webpage);

            Assert.That(result.Length, Is.GreaterThan(0));
        }
    }

    [TestFixture]
    public class WhenAnInvalidUrlIsProvided
    {
        [Test]
        [ExpectedException(typeof(PageDownloaderException), ExpectedMessage = "invalid url")]
        public void Then_it_should_throw_an_PageDownloaderException()
        {
            PageDownloader.Download("htzp://www.asdasda.com");

        }
    }
}
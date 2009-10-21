using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.ZuneWebsiteScraper
{
    [TestFixture]
    public class TidyHtmlTests
    {
        [Test]
        public void Should_be_able_to_repair_an_invalid_html_file_then_read_the_media_details()
        {
            var input = new StreamReader("invalidalbumlistwebpage.xml").ReadToEnd();

            string output = TidyHtml.Clean(input);


            AlbumMediaIDScraper albumMediaIDScraper = new AlbumMediaIDScraper(output);

            Dictionary<string, string> dictionary = albumMediaIDScraper.Scrape();

            Assert.That(dictionary.Count, Is.GreaterThan(0));
        }

    }
}
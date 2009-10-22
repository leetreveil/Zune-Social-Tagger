using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using System.IO;

namespace ZuneSocialTagger.IntegrationTests.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenAValidZuneAlbumWebpageIsProvided
    {
        private const string PathToFile = "ZuneWebsiteScraper/validalbumlistwebpage.xml";
        private string _fileData;

        public WhenAValidZuneAlbumWebpageIsProvided()
        {
            _fileData = new StreamReader(PathToFile).ReadToEnd();
        }

        [Test]
        public void Then_it_should_be_able_to_get_a_dictionary_of_song_titles_and_zuneMediaID_from_an_album_document()
        {
            var albumMediaIDScraper = new ZuneAlbumWebpageScraper(_fileData);

            var songs = albumMediaIDScraper.GetSongTitleAndIDs();

            Assert.That(songs.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_first_song_from_the_webpage()
        {
            var expectedOutput = new KeyValuePair<string, Guid>("We Were Aborted", new Guid("39b9f201-0100-11db-89ca-0019b92a3933"));

            var albumMediaIDScraper = new ZuneAlbumWebpageScraper(_fileData);

            var songs = albumMediaIDScraper.GetSongTitleAndIDs();

            Assert.That(songs.First(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_AlbumArtistID()
        {
            Guid albumArtistID = new Guid("00710a00-0600-11db-89ca-0019b92a3933");

            var scraper = new ZuneAlbumWebpageScraper(_fileData);

            Assert.That(scraper.ScrapeAlbumArtistID(), Is.EqualTo(albumArtistID));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_out_the_ZuneAlbumMediaID()
        {
            Guid zuneAlbumMediaID = new Guid("37b9f201-0100-11db-89ca-0019b92a3933");

            var scraper = new ZuneAlbumWebpageScraper(_fileData);

            Assert.That(scraper.ScrapeAlbumMediaID(), Is.EqualTo(zuneAlbumMediaID));
        }
    }

    /// <summary>
    /// This is ensuring that a user cannot pass www.google.com in instead of a www.zune.net/... webpage
    /// </summary>
    [TestFixture]
    public class WhenAInvalidZuneWebpageIsProvided
    {
        private const string PathToFile = "ZuneWebsiteScraper/nonzunewebpage.xml";
        private string _fileData;

        public WhenAInvalidZuneWebpageIsProvided()
        {
            _fileData = new StreamReader(PathToFile).ReadToEnd();
        }

        [Test]
        [ExpectedException(typeof(WebpageParseException))]
        public void Then_it_should_throw_a_PageDownloaderException()
        {
            new ZuneAlbumWebpageScraper(_fileData);
        }
    }
}
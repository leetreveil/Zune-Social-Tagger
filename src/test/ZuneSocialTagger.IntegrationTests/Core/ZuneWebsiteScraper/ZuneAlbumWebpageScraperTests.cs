using System;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using System.IO;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenAValidZuneAlbumWebpageIsProvided
    {
        private const string PathToFile = "SampleData/validalbumlistwebpage.xml";
        private string _fileData;

        public WhenAValidZuneAlbumWebpageIsProvided()
        {
            _fileData = new StreamReader(PathToFile).ReadToEnd();
        }

        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_song_titles_and_zuneMediaID_from_an_album_document()
        {
            var albumMediaIDScraper = new ZuneAlbumWebpageScraper(_fileData);

            var songs = albumMediaIDScraper.GetSongTitleAndIDs();

            Assert.That(songs.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_first_song_from_the_webpage()
        {
            var expectedOutput = new Song
                                     {
                                         Title = "We Were Aborted",
                                         Guid = new Guid("39b9f201-0100-11db-89ca-0019b92a3933")
                                     };

            var albumMediaIDScraper = new ZuneAlbumWebpageScraper(_fileData);

            var songs = albumMediaIDScraper.GetSongTitleAndIDs();

            Assert.That(songs.First().Guid,Is.EqualTo(expectedOutput.Guid));
            Assert.That(songs.First().Title, Is.EqualTo(expectedOutput.Title));
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


        [Test]
        public void Then_it_should_be_able_to_scrape_the_album_artist()
        {
            var scraper = new ZuneAlbumWebpageScraper(_fileData);

            string artist = scraper.ScrapeAlbumArtist();

            Assert.That(artist,Is.EqualTo("The Cribs"));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_album_title()
        {
            var scraper = new ZuneAlbumWebpageScraper(_fileData);

            string albumTitle = scraper.ScrapeAlbumTitle();

            Assert.That(albumTitle, Is.EqualTo("Ignore The Ignorant"));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_albums_release_year()
        {
            var scraper = new ZuneAlbumWebpageScraper(_fileData);

            int releaseYear = scraper.ScrapeAlbumReleaseYear();

            Assert.That(releaseYear, Is.EqualTo(2009));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_url_to_the_albums_artwork()
        {
            var scraper = new ZuneAlbumWebpageScraper(_fileData);

            string expectedUrl =
                "http&#58;&#47;&#47;image.catalog.zune.net&#47;v3.0&#47;image&#47;37b9f201-0300-11db-89ca-0019b92a3933&#63;resize&#61;true&#38;width&#61;240&#38;height&#61;240";

            string url = scraper.ScrapeAlbumArtworkUrl();

            Assert.That(url,Is.EqualTo(expectedUrl));
        }


    }

    /// <summary>
    /// This is ensuring that a user cannot pass www.google.com in instead of a www.zune.net/... webpage
    /// </summary>
    [TestFixture]
    public class WhenAInvalidZuneWebpageIsProvided
    {
        private const string PathToFile = "SampleData/nonzunewebpage.xml";
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
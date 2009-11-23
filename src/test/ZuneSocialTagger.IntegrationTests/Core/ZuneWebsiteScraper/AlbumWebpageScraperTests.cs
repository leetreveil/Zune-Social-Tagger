using System;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.IO;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenAValidZuneAlbumWebpageIsProvided
    {
        private const string PathToFile = "SampleData/validalbumwebpage.xml";
        private string _fileData;

        public WhenAValidZuneAlbumWebpageIsProvided()
        {
            _fileData = new StreamReader(PathToFile).ReadToEnd();
        }

        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_song_titles_and_zuneMediaID_from_an_album_document()
        {
            var albumMediaIDScraper = new AlbumWebpageScraper(_fileData);

            var songs = albumMediaIDScraper.Scrape().SongTitlesAndMediaID;

            Assert.That(songs.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_first_song_from_the_webpage()
        {
            var expectedOutput = new SongGuid
                                     {
                                         Title = "We Were Aborted",
                                         Guid = new Guid("39b9f201-0100-11db-89ca-0019b92a3933")
                                     };

            var albumMediaIDScraper = new AlbumWebpageScraper(_fileData);

            var songs = albumMediaIDScraper.Scrape().SongTitlesAndMediaID;

            Assert.That(songs.First().Guid,Is.EqualTo(expectedOutput.Guid));
            Assert.That(songs.First().Title, Is.EqualTo(expectedOutput.Title));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_AlbumArtistID()
        {
            Guid albumArtistID = new Guid("00710a00-0600-11db-89ca-0019b92a3933");

            var scraper = new AlbumWebpageScraper(_fileData);

            Assert.That(scraper.Scrape().AlbumArtistID, Is.EqualTo(albumArtistID));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_out_the_ZuneAlbumMediaID()
        {
            Guid zuneAlbumMediaID = new Guid("37b9f201-0100-11db-89ca-0019b92a3933");

            var scraper = new AlbumWebpageScraper(_fileData);

            Assert.That(scraper.Scrape().AlbumMediaID, Is.EqualTo(zuneAlbumMediaID));
        }


        [Test]
        public void Then_it_should_be_able_to_scrape_the_album_artist()
        {
            var scraper = new AlbumWebpageScraper(_fileData);

            Assert.That(scraper.Scrape().AlbumArtist,Is.EqualTo("The Cribs"));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_album_title()
        {
            var scraper = new AlbumWebpageScraper(_fileData);

            Assert.That(scraper.Scrape().AlbumTitle, Is.EqualTo("Ignore The Ignorant"));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_albums_release_year()
        {
            var scraper = new AlbumWebpageScraper(_fileData);

            Assert.That(scraper.Scrape().AlbumReleaseYear, Is.EqualTo(2009));
        }

        [Test]
        public void Then_it_should_be_able_to_scrape_the_url_to_the_albums_artwork()
        {
            var scraper = new AlbumWebpageScraper(_fileData);

            string expectedUrl =
                "http&#58;&#47;&#47;image.catalog.zune.net&#47;v3.0&#47;image&#47;37b9f201-0300-11db-89ca-0019b92a3933&#63;resize&#61;true&#38;width&#61;240&#38;height&#61;240";

            Assert.That(scraper.Scrape().AlbumArtworkUrl,Is.EqualTo(expectedUrl));
        }

        [Test]
        public void Then_it_should_validate()
        {
            var scraper = new AlbumWebpageScraper(_fileData);

            Assert.That(scraper.Scrape().IsValid(),Is.True);

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
        public void Then_it_should_not_be_valid()
        {
            Assert.That(new AlbumWebpageScraper(_fileData).Scrape().IsValid(),Is.False);
        }
    }
}
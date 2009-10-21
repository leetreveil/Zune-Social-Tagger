using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using System.IO;

namespace ZuneSocialTagger.IntegrationTests.ZuneWebsiteScraper
{
    [TestFixture]
    public class AlbumMediaIDScraperTests
    {
        //TODO: Figure out how to get the artist title and artist guid from the same page
        private const string PathToFile = "validalbumlistwebpage.xml";
        private string _fileData;

        public AlbumMediaIDScraperTests()
        {
            _fileData = new StreamReader(PathToFile).ReadToEnd();
        }

        [Test]
        public void Should_be_able_to_get_a_dictionary_of_song_titles_and_zuneMediaID_from_an_album_document()
        {
            var albumMediaIDScraper = new AlbumMediaIDScraper(_fileData);

            var songs = albumMediaIDScraper.GetSongTitleAndIDs();

            Assert.That(songs.Count(),Is.GreaterThan(0));
        }

        [Test]
        public void Should_be_able_to_scrape_the_first_song_from_the_webpage()
        {
            string firstTrack = "We Were Aborted";
            Guid firstTracksZuneMediaID = new Guid("39b9f201-0100-11db-89ca-0019b92a3933");

            var expectedOutput = new KeyValuePair<string, Guid>(firstTrack, firstTracksZuneMediaID);

            var albumMediaIDScraper = new AlbumMediaIDScraper(_fileData);

            var songs = albumMediaIDScraper.GetSongTitleAndIDs();

            Assert.That(songs.First(),Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_be_able_to_scrape_the_AlbumArtistID()
        {
            Guid albumArtistID = new Guid("00710a00-0600-11db-89ca-0019b92a3933");

            var scraper = new AlbumMediaIDScraper(_fileData);

            Assert.That(scraper.ScrapeAlbumArtistID(), Is.EqualTo(albumArtistID));
        }

        [Test]
        public void Should_be_able_to_scrape_out_the_ZuneAlbumMediaID()
        {
            Guid zuneAlbumMediaID = new Guid("37b9f201-0100-11db-89ca-0019b92a3933");

            var scraper = new AlbumMediaIDScraper(_fileData);

            Assert.That(scraper.ScrapeAlbumMediaID(), Is.EqualTo(zuneAlbumMediaID));
        }
    }
}
using System.Collections.Generic;
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.ZuneWebsiteScraper
{
    [TestFixture]
    public class AlbumMediaIDScraperTests
    {
        private const string PathToFile = "validalbumlistwebpage.xml";

        [Test]
        public void Should_be_able_to_get_a_dictionary_of_song_titles_and_zuneMediaID_from_an_album_document()
        {
            AlbumMediaIDScraper albumMediaIDScraper = new AlbumMediaIDScraper(PathToFile);

            Dictionary<string,string> songs =  albumMediaIDScraper.Scrape();

            Assert.That(songs.Count,Is.GreaterThan(0));
        }

        [Test]
        public void Should_be_able_to_scrape_the_first_song_from_the_webpage()
        {
            string firstTrack = "We Were Aborted";
            string firstTracksZuneMediaID = "39b9f201-0100-11db-89ca-0019b92a3933";

            AlbumMediaIDScraper albumMediaIDScraper = new AlbumMediaIDScraper(PathToFile);

            Dictionary<string, string> songs = albumMediaIDScraper.Scrape();

            Assert.That(songs[firstTrack],Is.EqualTo(firstTracksZuneMediaID));
        }


        [Test]
        public void Should_be_able_to_convert_a_mediainfo_attribute_to_a_keypair()
        {
            string[] attribute = AlbumMediaIDScraper.GetIDAndSongNameFromMediaInfoAttribute(
                "41b9f201-0100-11db-89ca-0019b92a3933#song#Hari Kari");


            Assert.That(attribute[0], Is.EqualTo("41b9f201-0100-11db-89ca-0019b92a3933"));
            Assert.That(attribute[1], Is.EqualTo("Hari Kari"));
        }
    }
}
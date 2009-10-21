using System.Collections.Generic;
using NUnit.Framework;

namespace ZuneSocialTagger.IntegrationTests
{
    [TestFixture]
    public class AlbumMediaIDScraperTests
    {
        private const string AlbumPage =  "@http://social.zune.net/album/The-Cribs/Ignore-The-Ignorant/37b9f201-0100-11db-89ca-0019b92a3933/details";

        [Test]
        public void Should_be_able_to_get_a_dictionary_of_songs_from_an_album_and_their_ZuneMediaID()
        {
            AlbumMediaIDScraper albumMediaIDScraper = new AlbumMediaIDScraper(AlbumPage);

            Dictionary<string,string> songs =  albumMediaIDScraper.Scrape();
        }

        [Test]
        public void Should_be_able_to_scrape_the_first_song_from_the_webpage()
        {
            string firstTrack = "We Were Aborted";
            string firstTracksZuneMediaID = "39b9f201-0100-11db-89ca-0019b92a3933";

            AlbumMediaIDScraper albumMediaIDScraper = new AlbumMediaIDScraper(AlbumPage);

            Dictionary<string, string> songs = albumMediaIDScraper.Scrape();

            Assert.That(songs[firstTrack],Is.EqualTo(firstTracksZuneMediaID));
        }


    }
}
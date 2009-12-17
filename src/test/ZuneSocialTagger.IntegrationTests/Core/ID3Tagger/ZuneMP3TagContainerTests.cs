using System.Drawing;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenATagContainerIsLoaded
    {
        private const string FilePath = "SampleData/id3v2.3withartwork.mp3";

        [Test]
        public void Then_it_should_be_able_to_read_the_tracks_metadata()
        {
            var zuneMp3TagContainer = ZuneTagContainerFactory.GetContainer(FilePath);

            Track data = zuneMp3TagContainer.ReadMetaData();

            Assert.That(data.AlbumArtist, Is.EqualTo("Subkulture"));
            Assert.That(data.ContributingArtists.First(), Is.EqualTo("Subkulture"));
            Assert.That(data.AlbumName, Is.EqualTo("Erasus"));
            Assert.That(data.Title, Is.EqualTo("Erasus"));
            Assert.That(data.Year, Is.EqualTo("2007"));
            Assert.That(data.TrackNumber, Is.EqualTo(1));
        }
    }
}
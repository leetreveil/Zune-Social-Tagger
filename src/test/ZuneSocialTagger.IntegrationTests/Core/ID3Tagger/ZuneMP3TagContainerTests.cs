using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenATagContainerIsLoadedWithMediaIdsPresent
    {
        private const string FilePath = "SampleData/id3v2.3withartwork.mp3";

        [Test]
        public void Then_it_should_be_able_to_read_the_tracks_metadata()
        {
            var zuneMp3TagContainer = ZuneTagContainerFactory.GetContainer(FilePath);

            MetaData data = zuneMp3TagContainer.ReadMetaData();

            Assert.That(data.AlbumArtist, Is.EqualTo("Subkulture"));
            Assert.That(data.ContributingArtists.First(), Is.EqualTo("Subkulture"));
            Assert.That(data.AlbumName, Is.EqualTo("Erasus"));
            Assert.That(data.Title, Is.EqualTo("Erasus"));
            Assert.That(data.Year, Is.EqualTo("2007"));
            Assert.That(data.TrackNumber, Is.EqualTo("1"));
        }

        [Test]
        public void Then_it_should_be_able_to_remove_all_the_media_ids()
        {
            var zuneMp3TagContainer = (ZuneMP3TagContainer) ZuneTagContainerFactory.GetContainer(FilePath);

            zuneMp3TagContainer.RemoveZuneAttribute(ZuneAttributes.Artist);
            zuneMp3TagContainer.RemoveZuneAttribute(ZuneAttributes.Track);
            zuneMp3TagContainer.RemoveZuneAttribute(ZuneAttributes.Album);

            zuneMp3TagContainer.WriteToFile(FilePath);
 
            IEnumerable<ZuneAttribute> ids = ZuneTagContainerFactory.GetContainer(FilePath).ReadZuneAttributes();

            Assert.That(ids,Is.Empty);
        }

    }
}
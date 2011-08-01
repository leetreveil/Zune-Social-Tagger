using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.IO.ID3Tagger;

namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenATagContainerIsLoadedWithMediaIdsPresent
    {
        private const string FilePath = "SampleData/id3v2.3withartwork.mp3";

        [Test]
        public void Then_it_should_be_able_to_read_the_tracks_metadata()
        {
            var container = ZuneTagContainerFactory.GetContainer(FilePath);

            MetaData data = container.MetaData;

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
            var container = (ZuneMP3TagContainer) ZuneTagContainerFactory.GetContainer(FilePath);

            container.RemoveZuneAttribute(ZuneIds.Artist);
            container.RemoveZuneAttribute(ZuneIds.Track);
            container.RemoveZuneAttribute(ZuneIds.Album);

            Assert.That(container.ZuneAttributes, Is.Empty);
        }

    }
}
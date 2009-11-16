using System.Drawing;
using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenATagContainerIsLoadedWithAPictureFrame
    {
        private const string FilePath = "SampleData/id3v2.3withartwork.mp3";

        [Test]
        public void Then_it_should_be_able_to_load_the_picture_as_a_bitmap()
        {
            ZuneTagContainer container = ZuneTagContainerFactory.GetContainer(FilePath);

            MetaData data = container.ReadMetaData();

            Assert.That(data.Picture,Is.Not.Null);
            Assert.That(data.Picture.Size,Is.EqualTo(new Size(600,600)));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_tracks_metadata()
        {
            ZuneTagContainer zuneTagContainer = ZuneTagContainerFactory.GetContainer(FilePath);

            MetaData data = zuneTagContainer.ReadMetaData();

            Assert.That(data.AlbumArtist, Is.EqualTo("Subkulture"));
            Assert.That(data.AlbumTitle, Is.EqualTo("Erasus"));
            Assert.That(data.SongTitle, Is.EqualTo("Erasus"));
            Assert.That(data.Year, Is.EqualTo("2007"));
            Assert.That(data.Index, Is.EqualTo("1"));
        }
    }
}
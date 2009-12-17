using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;


namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenProvidedWithAFileWithAnID3Tag
    {
        private const string FilePath = "SampleData/id3v2.3withartwork.mp3";

        [Test]
        public void Then_it_should_load_a_Zune_Tag_Container_for_that_file()
        {
            var zuneMp3TagContainer = ZuneTagContainerFactory.GetContainer(FilePath);

            Assert.That(zuneMp3TagContainer, Is.Not.Null);
        }

    }

    [TestFixture]
    public class WhenProvidedWithAFileWithJustID3V1Point1
    {
        private const string FilePath = "SampleData/id3v1.1.mp3";

        [Test]
        [ExpectedException(typeof(ID3Tag.ID3TagException))]
        public void Then_it_should_throw_a_not_supported_exception()
        {
            ZuneTagContainerFactory.GetContainer(FilePath);
        }
    }

    /// <summary>
    /// Special case for files with id3v2.2
    /// </summary>
    [TestFixture]
    public class WhenProvidedWithAFileWithAnID3TagWithMajorRevision2Point2
    {
        private const string FilePath = "SampleData/id3v2.2.mp3";

        [Test]
        [ExpectedException(typeof (ID3Tag.ID3TagException), ExpectedMessage = "This major revision is not supported!")]
        public void Then_it_should_get_an_empty_container_at_the_id_3_version_we_require()
        {
            var zuneMp3TagContainer = ZuneTagContainerFactory.GetContainer(FilePath);

            Assert.That(zuneMp3TagContainer, Is.Not.Null);
        }
    }
}
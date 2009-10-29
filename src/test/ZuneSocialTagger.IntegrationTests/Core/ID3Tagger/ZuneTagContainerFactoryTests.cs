using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;


namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
     public class WhenProvidedWithAFileWithAnID3Tag
     {
        private const string FilePath =
            "SampleData/Editors - In This Light And On This Evening/01 - In This Light And On This Evening.mp3";

        [Test]
        public void Then_it_should_load_a_Zune_Tag_Container_for_that_file()
        {
            ZuneTagContainer zuneTagContainer = ZuneTagContainerFactory.GetContainer(FilePath);
            
            Assert.That(zuneTagContainer,Is.Not.Null);       
        }
     }

    //[TestFixture]
    // public class WhenProvidedWithAFileWithNoID3Tag
    // {

    // }

    //[TestFixture]
    // public class WhenProvidedWithAFileWithAnID3TagBelowTheMinimumVersion
    // {

    // }
}
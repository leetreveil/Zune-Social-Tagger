using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenProvidedWithAnArtistToSearchForAndAPageNumber
    {
        [Test]
        public void Then_it_should_be_able_to_create_the_url_to_the_first_page()
        {
            string result = ZuneArtistSearchUrlGenerator.CreateUrl("Pendulum", 1);

            Assert.That(result,
                        Is.EqualTo(
                            "http://social.zune.net/frag/AlbumSearchBlock/?PageIndex=1&IsFullListView=true&keyword=Pendulum&PageSize=&blockName=AlbumSearchBlock&id=_searchAlbums&"));
        }

        [Test]
        public void Then_it_should_be_able_to_create_the_url_to_the_n_page()
        {
            for (int i = 0; i < 10; i++)
            {
                string result = ZuneArtistSearchUrlGenerator.CreateUrl("Pendulum", i);

                Assert.That(result,
                            Is.EqualTo(
                                "http://social.zune.net/frag/AlbumSearchBlock/?PageIndex="+ i+ "&IsFullListView=true&keyword=Pendulum&PageSize=&blockName=AlbumSearchBlock&id=_searchAlbums&"));
            }
        }

        [Test]
        public void Then_it_should_be_able_to_create_the_url_with_the_default_page_no_which_is_1()
        {
            string result = ZuneArtistSearchUrlGenerator.CreateUrl("Pendulum");

            Assert.That(result,
            Is.EqualTo(
                "http://social.zune.net/frag/AlbumSearchBlock/?PageIndex=1&IsFullListView=true&keyword=Pendulum&PageSize=&blockName=AlbumSearchBlock&id=_searchAlbums&"));
        }

    }
}
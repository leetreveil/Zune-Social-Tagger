using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenProviedWithAnAlbumSearchWebPage
    {
        private const string PathToFile = "SampleData/albumsearchresult.xml";
        private readonly string _fileData;

        public WhenProviedWithAnAlbumSearchWebPage()
        {
            _fileData = new StreamReader(PathToFile).ReadToEnd();
        }

        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_18_albums()
        {
            var zuneArtistSearchScraper = new AlbumSearchScraper(_fileData);
            IEnumerable<AlbumSearchResult> results = zuneArtistSearchScraper.ScrapeAlbums();


            Assert.That(results.Count(),Is.EqualTo(18));
            //i know that the first result in the xml file is pendulum
            Assert.That(results.First().Artist, Is.EqualTo("Pendulum"));
        }

        [Test]
        public void Then_the_first_result_should_be()
        {
            var zuneArtistSearchScraper = new AlbumSearchScraper(_fileData);
            IEnumerable<AlbumSearchResult> results = zuneArtistSearchScraper.ScrapeAlbums();

            //i know that the first result in the xml file is pendulum
            Assert.That(results.First().Artist, Is.EqualTo("Pendulum"));
            Assert.That(results.First().Title, Is.EqualTo("In Silico"));
            Assert.That(results.First().Url,
            Is.EqualTo(
                "http://social.zune.net/album/Pendulum/In-Silico/7510d300-0100-11db-89ca-0019b92a3933/details"));
        }

        [Test]
        public void Then_it_should_contain()
        {
            var zuneArtistSearchScraper = new AlbumSearchScraper(_fileData);
            IEnumerable<AlbumSearchResult> results = zuneArtistSearchScraper.ScrapeAlbums();

            Assert.That(results.Where(srch => srch.Artist == "Mary Anne Czapla").FirstOrDefault(), Is.Not.Null, "could not find Mary Anne Czapla");
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_total_number_of_albums_available()
        {
            var zuneArtistSearchScraper = new AlbumSearchScraper(_fileData);

            int result = zuneArtistSearchScraper.ScrapeAlbumCountAcrossAllPages();

            Assert.That(result, Is.EqualTo(38));
        }

    }

    [TestFixture]
    public class WhenProvidedWithAnAlbumSearchWebpageFromARealUrl
    {
        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_all_the_album_data_for_the_search()
        {
            string pageData = PageDownloader.Download(AlbumSearchUrlGenerator.CreateUrl("Pendulum"));

            var scraper = new AlbumSearchScraper(pageData);

            Assert.That(scraper.ScrapeAlbums().Count(),Is.GreaterThan(0));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_total_number_of_albums_available()
        {
            string pageData = PageDownloader.Download(AlbumSearchUrlGenerator.CreateUrl("Pendulum"));

            var scraper = new AlbumSearchScraper(pageData);

            Assert.That(scraper.ScrapeAlbumCountAcrossAllPages(), Is.GreaterThan(0));
        }


        //NOTE: this test could fail in the future because the datasource could change i.e albums could be added
        [Test]
        public void Then_it_should_be_able_to_get_the_first_albums_details_which_is_creedance()
        {
            string pageData = PageDownloader.Download(AlbumSearchUrlGenerator.CreateUrl("Pendulum"));

            var scraper = new AlbumSearchScraper(pageData);

            IEnumerable<AlbumSearchResult> albumSearchResults = scraper.ScrapeAlbums();

            Assert.That(albumSearchResults.First().Artist, Is.EqualTo("Creedence Clearwater Revival"));
            Assert.That(albumSearchResults.First().Title, Is.EqualTo("Pendulum"));
            Assert.That(albumSearchResults.First().Url, Is.EqualTo("http://social.zune.net/album/Creedence-Clearwater-Revival/Pendulum/abecf900-0100-11db-89ca-0019b92a3933/details"));
        }

        [Test]
        public void Then_it_should_contain()
        {
            string pageData = PageDownloader.Download(AlbumSearchUrlGenerator.CreateUrl("Pendulum"));

            var scraper = new AlbumSearchScraper(pageData);

            IEnumerable<AlbumSearchResult> albumSearchResults = scraper.ScrapeAlbums();

            AlbumSearchResult album = albumSearchResults.Where(x => x.Artist == "Mike Taylor").FirstOrDefault();

            Assert.That(album, Is.Not.Null, "Could not find Mike Taylor");

            Assert.That(album.Artist, Is.EqualTo("Mike Taylor"));
            Assert.That(album.Title, Is.EqualTo("Pendulum"));
            Assert.That(album.Url, Is.EqualTo("http://social.zune.net/album/Mike-Taylor/Pendulum/52540200-0400-11db-89ca-0019b92a3933/details"));
        }

    }
}
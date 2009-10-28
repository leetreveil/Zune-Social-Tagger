using System;
using System.Collections;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenSearchingForTheArtistPendulum
    {
        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_all_albums_matching_the_search()
        {
            var search = new ZuneArtistSearch();

            IEnumerable<AlbumSearchResult> results =  search.SearchFor("Pendulum");

            Assert.That(results.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Then_it_should_be_able_to_find_in_silico_in_the_search_result()
        {
            var search = new ZuneArtistSearch();

            IEnumerable<AlbumSearchResult> results = search.SearchFor("Pendulum");

            AlbumSearchResult result = results.Where(album => album.Title == "In Silico").FirstOrDefault();

            Assert.That(result.Artist,Is.EqualTo("Pendulum"));
            Assert.That(result.Title, Is.EqualTo("In Silico"));
            Assert.That(result.Url, Is.EqualTo("http://social.zune.net/album/Pendulum/In-Silico/7510d300-0100-11db-89ca-0019b92a3933/details"));
        }

        [Test]
        public void Then_nothing_should_be_repeated_twice()
        {
            var search = new ZuneArtistSearch();

            IEnumerable<AlbumSearchResult> results = search.SearchFor("Pendulum");

            var result =
                results.Where(album => album.Title == "Pendulum" && album.Artist == "Creedence Clearwater Revival");

            Assert.That(result.Count(),Is.EqualTo(1),"Found the same album twice");
        }


    }

    public class ZuneArtistSearch
    {
        public IEnumerable<AlbumSearchResult> SearchFor(string artist)
        {
            var tempList = new List<AlbumSearchResult>();
            string url = ZuneArtistSearchUrlGenerator.CreateUrl(artist);

            string firstAlbumPage = PageDownloader.Download(url);

            var scraper = new ZuneArtistSearchScraper(firstAlbumPage);
  
            var combiner = new ZuneArtistSearchScraperCombiner();

            int pageCount = combiner.GetPageCount(scraper.ScrapeAlbumCountAcrossAllPages());

            //TODO: fix this
            //NOTE: this is hugely inefficient because we are downloading the first page twice!
            for (int i = 0; i < pageCount; i++)
            {
                // + 1 because the url starts at 1 and i starts at 0
                string page = ZuneArtistSearchUrlGenerator.CreateUrl(artist, i +1);
                var newPageScraper =
                    new ZuneArtistSearchScraper(
                        PageDownloader.Download(page));

                tempList.AddRange(newPageScraper.ScrapeAlbums());
            }

            return tempList;
        }
    }
}
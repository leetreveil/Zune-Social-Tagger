using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenSearchingForTheArtistPendulum : AsyncTesting
    {
        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_all_albums_matching_the_search()
        {
            IEnumerable<AlbumSearchResult> results =  AlbumSearch.SearchFor("Pendulum");

            Assert.That(results.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Then_it_should_be_able_to_find_in_silico_in_the_search_result()
        {
            IEnumerable<AlbumSearchResult> results = AlbumSearch.SearchFor("Pendulum");

            AlbumSearchResult result = results.Where(album => album.Title == "In Silico").FirstOrDefault();

            Assert.That(result.Artist,Is.EqualTo("Pendulum"));
            Assert.That(result.Title, Is.EqualTo("In Silico"));
            Assert.That(result.Url, Is.EqualTo("http://social.zune.net/album/Pendulum/In-Silico/7510d300-0100-11db-89ca-0019b92a3933/details"));
        }

        [Test]
        public void Then_nothing_should_be_repeated_twice()
        {
            IEnumerable<AlbumSearchResult> results = AlbumSearch.SearchFor("Pendulum");

            var result =
                results.Where(album => album.Title == "Pendulum" && album.Artist == "Creedence Clearwater Revival");

            Assert.That(result.Count(),Is.Not.GreaterThan(1),"Found the same album twice");
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_result_of_the_first_page_asyncronously()
        {
            var listOfResults = new List<AlbumSearchResult>();

            AlbumSearch.SearchForAsync("Pendulum", searchResults =>
                                                       {
                                                           listOfResults.AddRange(searchResults);
                                                           base.Set();
                                                       });

            base.WaitOne(4000, "did not first page");
            Assert.That(listOfResults.Count(), Is.EqualTo(20));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_result_of_all_the_pages_asyncronously()
        {
            var listOfResults = new List<AlbumSearchResult>();

            AlbumSearch.SearchForAsync("Pendulum", searchResults =>
            {
                listOfResults.AddRange(searchResults);
                base.Set();
            });

            base.WaitOne(4000, "did not get first page");
            base.WaitOne(4000, "did not get second page");
            Assert.That(listOfResults.Count(), Is.EqualTo(38));
        }

        [Test]
        public void Then_it_should_raise_a_completed_event_when_all_pages_have_been_downloaded_asyncronously()
        {
            var listOfResults = new List<AlbumSearchResult>();

            AlbumSearch.SearchForAsyncCompleted += (() => base.Set());

            AlbumSearch.SearchForAsync("Pendulum", searchResults =>
            {
                listOfResults.AddRange(searchResults);
                base.Set();
            });

            base.WaitOne(4000, "did not get first page");
            base.WaitOne(4000, "did not get second page");
            base.WaitOneWith500MsTimeoutAnd("did not get completed event");
            Assert.That(listOfResults.Count(), Is.EqualTo(38));
        }

    }

}
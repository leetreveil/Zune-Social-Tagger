using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.Xml;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsiteScraper
{
    [TestFixture]
    public class WhenSearchingForTheArtistPendulumInADocument
    {
        private static string _file = "SampleData/albumsearchresult.xml";

        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_all_albums_matching_the_search()
        {
            IEnumerable<AlbumSearchResult> enumerable = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file));
            Assert.That(enumerable.Count(),Is.EqualTo(38));
        }

        [Test]
        public void Then_each_item_should_contain_a_guid_and_the_album_title_and_the_album_artst()
        {
            IEnumerable<AlbumSearchResult> result = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file));

            bool areAnyNull = result.All(arg => arg.Guid == Guid.Empty || String.IsNullOrEmpty(arg.Title) || String.IsNullOrEmpty(arg.Artist));

            Assert.That(areAnyNull,Is.False);
        }

        [Test]
        public void Then_the_first_result_should_equal()
        {
            AlbumSearchResult firstResult = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file)).First();

            Assert.That(firstResult.Guid, Is.EqualTo(new Guid("abecf900-0100-11db-89ca-0019b92a3933")));
            Assert.That(firstResult.Artist, Is.EqualTo("Creedence Clearwater Revival"));
            Assert.That(firstResult.Title, Is.EqualTo("Pendulum"));
        }
    }
}
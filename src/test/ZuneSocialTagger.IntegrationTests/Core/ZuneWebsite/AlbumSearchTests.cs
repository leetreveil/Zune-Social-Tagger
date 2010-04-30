using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.Xml;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsite
{
    [TestFixture]
    public class WhenSearchingForTheArtistPendulumInADocument
    {
        private static string _file = "SampleData/albumsearch.xml";

        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_all_albums_matching_the_search()
        {
            IEnumerable<Album> enumerable = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file));
            Assert.That(enumerable.Count(),Is.EqualTo(38));
        }

        [Test]
        public void Then_each_item_should_contain_a_guid_and_the_album_title_and_the_album_artst()
        {
            IEnumerable<Album> result = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file));

            bool areAnyNull = result.All(arg => arg.AlbumMediaId == Guid.Empty || String.IsNullOrEmpty(arg.Title) || String.IsNullOrEmpty(arg.Artist));

            Assert.That(areAnyNull,Is.False);
        }

        [Test]
        public void Then_the_first_result_should_equal()
        {
            Album firstResult = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file)).First();

            Assert.That(firstResult.AlbumMediaId, Is.EqualTo(new Guid("abecf900-0100-11db-89ca-0019b92a3933")));
            Assert.That(firstResult.Artist, Is.EqualTo("Creedence Clearwater Revival"));
            Assert.That(firstResult.Title, Is.EqualTo("Pendulum"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_url_to_the_image_for_the_album()
        {
            Album result = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file)).First();

            Assert.That(result.ArtworkUrl, Is.EqualTo(
                                               "http://image.catalog.zune.net/v3.0/image/abecf900-0300-11db-89ca-0019b92a3933?width=60&height=60"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_release_year()
        {
            Album result = AlbumSearch.ReadFromXmlDocument(XmlReader.Create(_file)).First();

            Assert.That(result.ReleaseYear, Is.EqualTo("1970"));
        }
    }
}
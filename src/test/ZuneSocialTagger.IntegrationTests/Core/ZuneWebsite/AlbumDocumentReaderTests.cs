using System;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.Xml;

namespace ZuneSocialTagger.IntegrationTests.Core.ZuneWebsite
{
    [TestFixture]
    public class WhenAZuneAlbumDetailsXmlFileIsLoaded
    {
        private string _pathToDoc = "SampleData/albumdetails.xml";

        [Test]
        public void Then_it_should_be_able_to_get_the_title_of_the_album()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MetaData.AlbumName, Is.EqualTo("Reservoir Dogs"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_album_artist()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MetaData.AlbumArtist, Is.EqualTo("Movie Soundtracks"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_release_year()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MetaData.Year, Is.EqualTo("1992"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_url_to_the_artwork()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().ArtworkUrl, Is.EqualTo("http://image.catalog.zune.net/v3.0/image/9f3c6f4b-7787-dc11-883e-0019b9ef2915?width=234&height=320"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_a_list_of_tracks()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().Count(),Is.EqualTo(16));
        }

        [Test]
        public void Then_the_first_track_should_have_a_title()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MetaData.Title, Is.EqualTo("And Now Little Green Bag... (Dialogue)"));
        }

        [Test]
        public void Then_the_first_track_should_have_a_media_id()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MediaID, Is.EqualTo(new Guid("9ea52600-0500-11db-89ca-0019b92a3933")));
        }

        [Test]
        public void Then_the_first_track_should_have_an_album_media_id()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().AlbumMediaID,Is.EqualTo(new Guid("13ce0100-0400-11db-89ca-0019b92a3933")));
        }


        [Test]
        public void Then_the_first_track_should_have_an_album_artist()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MetaData.AlbumArtist, Is.EqualTo("Movie Soundtracks"));
        }

        [Test]
        public void Then_the_first_track_should_have_a_artist_media_id()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().ArtistMediaID, Is.EqualTo(new Guid("fd000000-0600-11db-89ca-0019b92a3933")));
        }

        [Test]
        public void Then_each_track_should_have_a_track_number()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            var album = docReader.Read();
            Assert.That(album.First().MetaData.TrackNumber, Is.EqualTo("1"));
            Assert.That(album.Last().MetaData.TrackNumber, Is.EqualTo("16"));
        }

        [Test]
        public void Then_the_first_track_should_have_three_contributing_artists()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Track firstTrack = docReader.Read().First();

            Assert.That(firstTrack.MetaData.ContributingArtists.ElementAt(0), Is.EqualTo("Steven Wright"));
            Assert.That(firstTrack.MetaData.ContributingArtists.ElementAt(1), Is.EqualTo("Quentin Tarantino"));
            Assert.That(firstTrack.MetaData.ContributingArtists.ElementAt(2), Is.EqualTo("Some Artist"));
        }

        [Test]
        public void Then_the_first_track_should_have_a_genre()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            var album = docReader.Read();

            Assert.That(album.First().MetaData.Genre, Is.EqualTo("Pop"));
        }

        [Test]
        public void Then_the_first_track_should_have_a_disc_number()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            var album = docReader.Read();

            Assert.That(album.First().MetaData.DiscNumber, Is.EqualTo("1"));
        }
    }

    [TestFixture]
    public class WhenAnAlbumDetailsFileIsLoadedWithMissingData
    {
        private string _pathToDoc = "SampleData/albumdetailswithmissingdata.xml";

        [Test]
        public void Then_it_should_return_null_or_empty_and_not_fail_when_reading_release_year()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MetaData.Year,Is.Null);
        }

        [Test]
        public void Then_it_should_return_null_or_empty_and_not_fail_when_reading_album_artist()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));

            Assert.That(docReader.Read().First().MetaData.AlbumArtist, Is.Null);
        }

        [Test]
        public void Then_it_should_return_just_the_primary_artist()
        {
            var docReader = new AlbumDocumentReader(XmlReader.Create(_pathToDoc));


            Assert.That(docReader.Read().First().MetaData.ContributingArtists.First(),Is.EqualTo("Randall Hall"));
        }

    }
}
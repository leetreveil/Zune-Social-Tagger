using System;
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.Collections.Generic;

namespace ZuneSocialTagger.UnitTests.Core.ZuneWebsite
{
    [TestFixture]
    public class AlbumTests
    {
        [Test]
        public void Should_not_validate_if_album_media_id_is_null()
        {
            var album = new Album();

            Assert.That(album.IsValid,Is.False);
        }

        [Test]
        public void Should_not_validate_if_album_media_id_is_not_null()
        {
            var album = new Album();

            album.AlbumMediaID = Guid.NewGuid();

            Assert.That(album.IsValid, Is.False);
        }

        [Test]
        public void Should_not_validate_if_album_media_id_is_not_null_and_tracks_is_just_an_empty_list()
        {
            var album = new Album();

            album.AlbumMediaID = Guid.NewGuid();
            album.Tracks = new List<Track>();

            Assert.That(album.IsValid, Is.False);
        }

        [Test]
        public void Should_validate_if_album_media_id_is_correct_and_tracks_are()
        {
            var album = new Album();

            album.AlbumMediaID = Guid.NewGuid();
            album.Tracks = new List<Track> {new Track {ArtistMediaID = Guid.NewGuid(),
                                                       MediaID = Guid.NewGuid(),
                                                       Title = "hello"}};

            Assert.That(album.IsValid,Is.True);
        }

        [Test]
        public void Should_be_able_to_validate_the_metadata_if_all_metadata_is_present()
        {
            var album = new Album() {Artist = "ha", ReleaseYear = 2009, Title = "ha"};

            Assert.That(album.HasAllMetaData,Is.True);
        }

        [Test]
        public void Should_be_able_to_validate_the_metadata_if_any_metadata_is_missing()
        {
            var album = new Album() { ReleaseYear = 2009, Title = "ha" };

            Assert.That(album.HasAllMetaData, Is.False);
         
        }
    }
}
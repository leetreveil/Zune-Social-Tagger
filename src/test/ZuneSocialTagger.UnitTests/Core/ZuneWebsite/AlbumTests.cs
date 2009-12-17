using System;
using NUnit.Framework;
using ZuneSocialTagger.Core.ZuneWebsite;

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
        public void Should_validate_if_album_media_id_is_correct()
        {
            var album = new Album();

            album.AlbumMediaID = Guid.NewGuid();

            Assert.That(album.IsValid,Is.True);
        }
    }
}
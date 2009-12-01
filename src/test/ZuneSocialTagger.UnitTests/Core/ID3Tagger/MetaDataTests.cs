using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    [TestFixture]
    public class MetaDataTests
    {
        [Test]
        public void Should_be_able_to_validate_a_metadata_object()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumTitle = "Forever",
                ContributingArtist = "U2/AFI",
                DiscNumber = "1/1",
                Genre = "Pop",
                SongTitle = "Wallet",
                TrackNumber = "2",
                Year = "2009"
            };

            Assert.That(metaData.IsValid,Is.True);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_year_missing()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumTitle = "Forever",
                ContributingArtist = "U2/AFI",
                DiscNumber = "1/1",
                Genre = "Pop",
                SongTitle = "Wallet",
                TrackNumber = "2",
            };

            Assert.That(metaData.IsValid,Is.False);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_track_number_missing()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumTitle = "Forever",
                ContributingArtist = "U2/AFI",
                DiscNumber = "1/1",
                Genre = "Pop",
                SongTitle = "Wallet",
            };

            Assert.That(metaData.IsValid, Is.False);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_song_title_missing()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumTitle = "Forever",
                ContributingArtist = "U2/AFI",
                DiscNumber = "1/1",
                Genre = "Pop",
            };

            Assert.That(metaData.IsValid, Is.False);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_genre_missing()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumTitle = "Forever",
                ContributingArtist = "U2/AFI",
                DiscNumber = "1/1",
            };

            Assert.That(metaData.IsValid, Is.False);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_disc_number_missing()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumTitle = "Forever",
                ContributingArtist = "U2/AFI",
            };

            Assert.That(metaData.IsValid, Is.False);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_contributing_artist_missing()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumTitle = "Forever",
            };

            Assert.That(metaData.IsValid, Is.False);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_album_title_missing()
        {
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
            };

            Assert.That(metaData.IsValid, Is.False);
        }

        [Test]
        public void Should_not_be_able_to_validate_a_metadata_object_with_everything_missing()
        {
            var metaData = new MetaData();

            Assert.That(metaData.IsValid, Is.False);
        }



    }
}
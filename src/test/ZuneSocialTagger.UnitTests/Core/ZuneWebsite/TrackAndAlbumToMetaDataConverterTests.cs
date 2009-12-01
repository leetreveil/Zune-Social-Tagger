using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsite;
using System.Collections.Generic;


namespace ZuneSocialTagger.UnitTests.Core.ZuneWebsite
{
    [TestFixture]
    public class TrackAndAlbumToMetaDataConverterTests
    {
        [Test]
        public void Should_be_able_to_take_a_track_and_album_and_convert_it_to_a_valid_metadata_object()
        {
            var album = new Album {Artist = "Various Artists", ReleaseYear = 2009, Title = "Ocra"};

            var track = new Track
                            {
                                Artist = "Okenfold",
                                ContributingArtists = new List<string>() {"Pendulum", "AFI"},
                                DiscNumber = 1,
                                Genre = "Rock",
                                Title = "Somewhere",
                                TrackNumber = 3
                            };

            var converter = new TrackAndAlbumToMetaDataConverter(album, track);


            MetaData expectedOutput = new MetaData()
                                          {
                                              AlbumArtist = "Various Artists",
                                              AlbumTitle = "Ocra",
                                              ContributingArtist = "Okenfold/Pendulum/AFI",
                                              DiscNumber = "1/1",
                                              Genre = "Rock",
                                              SongTitle = "Somewhere",
                                              TrackNumber = "3",
                                              Year = "2009"
                                          };

            Assert.That(converter.CanConvert, Is.True);

            MetaData converted = converter.Convert();

            Assert.That(converted.AlbumArtist, Is.EqualTo(expectedOutput.AlbumArtist));
            Assert.That(converted.AlbumTitle, Is.EqualTo(expectedOutput.AlbumTitle));
            Assert.That(converted.ContributingArtist, Is.EqualTo(expectedOutput.ContributingArtist));
            Assert.That(converted.DiscNumber, Is.EqualTo(expectedOutput.DiscNumber));
            Assert.That(converted.Genre, Is.EqualTo(expectedOutput.Genre));
            Assert.That(converted.SongTitle, Is.EqualTo(expectedOutput.SongTitle));
            Assert.That(converted.TrackNumber, Is.EqualTo(expectedOutput.TrackNumber));
            Assert.That(converted.Year, Is.EqualTo(expectedOutput.Year));
        }

        [Test]
        public void Should_not_be_able_to_convert_a_track_and_album_to_a_metadata_object_if_they_are_missing_information()
        {
            var album = new Album {ReleaseYear = 2009, Title = "Ocra"};

            var track = new Track
                            {
                                Artist = "Okenfold",
                                ContributingArtists = new List<string>() {"Pendulum", "AFI"},
                                DiscNumber = 1,
                                Genre = "Rock",
                                Title = "Somewhere",
                                TrackNumber = 3
                            };

            var converter = new TrackAndAlbumToMetaDataConverter(album, track);

            Assert.That(converter.CanConvert, Is.False);
        }
    }
}
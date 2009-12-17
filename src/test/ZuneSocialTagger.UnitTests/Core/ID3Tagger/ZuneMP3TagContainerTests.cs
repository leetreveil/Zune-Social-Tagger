using System;
using System.Collections.Generic;
using System.Linq;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenATagContainerIsLoadedWithTheCorrectMediaIdsPresent
    {
        [Test]
        public void Then_it_should_have_3_items()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            IEnumerable<MediaIdGuid> ids = container.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumArtistMediaId()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            MediaIdGuid result =
                container.ReadMediaIds().Where(x => x.MediaId == MediaIds.ZuneAlbumArtistMediaID).First();

            Assert.That(result.MediaId, Is.EqualTo(MediaIds.ZuneAlbumArtistMediaID));
            Assert.That(result.Guid, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumMediaId()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();
            string mediaId = MediaIds.ZuneAlbumArtistMediaID;

            MediaIdGuid result = container.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumAMediaId()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();
            string mediaId = MediaIds.ZuneMediaID;

            MediaIdGuid result = container.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_not_be_able_to_write_the_same_media_Id_to_the_container()
        {
            ZuneMP3TagContainer container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            var mediaIdGuid = new MediaIdGuid(MediaIds.ZuneMediaID, ZuneMP3TagContainerTestHelpers.SomeGuid);

            container.Add(mediaIdGuid);

            //we know that there are 3 items in the container so there should be no more
            Assert.That(container.GetContainer().Count, Is.EqualTo(3));
        }
    }


    [TestFixture]
    public class WhenATagContainerIsLoadedWithNoMediaIdsPresent
    {
        [Test]
        public void Then_the_read_media_ids_method_should_return_0()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateEmptyContainer();

            IEnumerable<MediaIdGuid> ids = container.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Then_it_should_be_able_to_add_a_mediaId_to_the_container()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateEmptyContainer();

            var mediaIdGuid = new MediaIdGuid(MediaIds.ZuneMediaID, ZuneMP3TagContainerTestHelpers.SomeGuid);

            container.Add(mediaIdGuid);

            var track = container.GetContainer().OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneMediaID).First();

            Assert.That(track.Owner, Is.EqualTo("ZuneMediaID"));
            Assert.That(new Guid(track.Data), Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }
    }

    [TestFixture]
    public class WhenATagContainerIsLoadedWithOnlyOneMediaIdButItIsIncorrect
    {
        [Test]
        public void Then_it_should_be_able_to_update_the_media_id_with_the_correct_guid()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithZuneAlbumartistMediaIDWithRandomGuid();

            var albumArtistMediaIdGuid = new MediaIdGuid(MediaIds.ZuneAlbumArtistMediaID,
                                                         ZuneMP3TagContainerTestHelpers.SomeGuid);

            container.Add(albumArtistMediaIdGuid);

            var artist = container.GetContainer().OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumArtistMediaID).First();

            Assert.That(new Guid(artist.Data), Is.EqualTo(albumArtistMediaIdGuid.Guid));
            Assert.That(container.GetContainer().Count(), Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class WhenATagContainerContainsMetaDataAboutTheTrack
    {
        [Test]
        public void Then_it_should_be_able_to_get_the_album_artist()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            Track metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumArtist, Is.EqualTo("Various Artists"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_contributing_artist()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            Track metaData = container.ReadMetaData();

            Assert.That(metaData.ContributingArtists.First(), Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeArtist));
        }


        [Test]
        public void Then_it_should_be_able_to_get_the_album_title()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            Track metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumName, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeAlbum));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_track_title()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            Track metaData = container.ReadMetaData();

            Assert.That(metaData.Title, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeTitle));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_release_year()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            var metaData = container.ReadMetaData();

            Assert.That(metaData.Year, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeYear));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_track_number()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            var metaData = container.ReadMetaData();

            Assert.That(metaData.TrackNumber, Is.EqualTo(2));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_disc_number()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            var metaData = container.ReadMetaData();

            Assert.That(metaData.DiscNumber, Is.EqualTo("2/2"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_genre()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            var metaData = container.ReadMetaData();

            Assert.That(metaData.Genre, Is.EqualTo("Pop"));
        }
    }

    [TestFixture]
    public class WhenATagContainerIsLoadedWithNoMetaData
    {
        [Test]
        public void Then_it_should_return_blank_values()
        {
            ZuneMP3TagContainer zuneMp3TagContainer = ZuneMP3TagContainerTestHelpers.CreateContainerWithNoMetaData();

            Track data = zuneMp3TagContainer.ReadMetaData();

            Assert.That(data.Year, Is.EqualTo(""));
            Assert.That(data.TrackNumber, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class WhenWritingMetaDataBackToContainerWithNoMetaData
    {

        [Test]
        public void Then_it_should_be_able_to_write_all_the_meta_data()
        {
            var metaData = new Track
                               {
                                        AlbumArtist = "Various Artists",
                                        AlbumName = "Forever",
                                        ContributingArtists = new List<string>{"U2","AFI"},
                                        DiscNumber = "1/1",
                                        Genre = "Pop",
                                        Title = "Wallet",
                                        TrackNumber = 2,
                                        Year = "2009"
                                 };

            ZuneMP3TagContainer zuneMp3TagContainer = ZuneMP3TagContainerTestHelpers.CreateContainerWithNoMetaData();

            zuneMp3TagContainer.WriteMetaData(metaData);

            Assert.That(zuneMp3TagContainer.ReadMetaData().AlbumArtist, Is.EqualTo(metaData.AlbumArtist));
            Assert.That(zuneMp3TagContainer.ReadMetaData().ContributingArtists, Is.EqualTo(metaData.ContributingArtists));
            Assert.That(zuneMp3TagContainer.ReadMetaData().AlbumName, Is.EqualTo(metaData.AlbumName));
            Assert.That(zuneMp3TagContainer.ReadMetaData().DiscNumber, Is.EqualTo(metaData.DiscNumber));
            Assert.That(zuneMp3TagContainer.ReadMetaData().Genre, Is.EqualTo(metaData.Genre));
            Assert.That(zuneMp3TagContainer.ReadMetaData().Title, Is.EqualTo(metaData.Title));
            Assert.That(zuneMp3TagContainer.ReadMetaData().TrackNumber, Is.EqualTo(metaData.TrackNumber));
            Assert.That(zuneMp3TagContainer.ReadMetaData().Year, Is.EqualTo(metaData.Year));
        }
    }

    [TestFixture]
    public class WhenWritingMetaDataBackToFileWithPreExistingMetaData
    {
        [Test]
        public void Then_it_should_update_any_existing_metadata()
        {
            var metaData = new Track()
            {
                AlbumArtist = "Various Artists",
                AlbumName = "Forever",
                ContributingArtists = new List<string> { "U2", "AFI" },
                DiscNumber = "1/1",
                Genre = "Pop",
                Title = "Wallet",
                TrackNumber = 2,
                Year = "2009"
            };

            ZuneMP3TagContainer zuneMp3TagContainer = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            zuneMp3TagContainer.WriteMetaData(metaData);

            Assert.That(zuneMp3TagContainer.ReadMetaData().AlbumArtist, Is.EqualTo(metaData.AlbumArtist));
            Assert.That(zuneMp3TagContainer.ReadMetaData().ContributingArtists, Is.EqualTo(metaData.ContributingArtists));
            Assert.That(zuneMp3TagContainer.ReadMetaData().AlbumName, Is.EqualTo(metaData.AlbumName));
            Assert.That(zuneMp3TagContainer.ReadMetaData().DiscNumber, Is.EqualTo(metaData.DiscNumber));
            Assert.That(zuneMp3TagContainer.ReadMetaData().Genre, Is.EqualTo(metaData.Genre));
            Assert.That(zuneMp3TagContainer.ReadMetaData().Title, Is.EqualTo(metaData.Title));
            Assert.That(zuneMp3TagContainer.ReadMetaData().TrackNumber, Is.EqualTo(metaData.TrackNumber));
            Assert.That(zuneMp3TagContainer.ReadMetaData().Year, Is.EqualTo(metaData.Year));
        }
    }
}
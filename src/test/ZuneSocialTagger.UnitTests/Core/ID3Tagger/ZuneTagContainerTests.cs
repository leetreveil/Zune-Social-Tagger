using System;
using System.Collections.Generic;
using System.Linq;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenATagContainerIsLoadedWithTheCorrectMediaIdsPresent
    {
        [Test]
        public void Then_it_should_have_3_items()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            IEnumerable<MediaIdGuid> ids = container.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumArtistMediaId()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            MediaIdGuid result =
                container.ReadMediaIds().Where(x => x.MediaId == MediaIds.ZuneAlbumArtistMediaID).First();

            Assert.That(result.MediaId, Is.EqualTo(MediaIds.ZuneAlbumArtistMediaID));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumMediaId()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithThreeZuneTags();
            string mediaId = MediaIds.ZuneAlbumArtistMediaID;

            MediaIdGuid result = container.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumAMediaId()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithThreeZuneTags();
            string mediaId = MediaIds.ZuneMediaID;

            MediaIdGuid result = container.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_not_be_able_to_write_the_same_media_Id_to_the_container()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            var mediaIdGuid = new MediaIdGuid(MediaIds.ZuneMediaID, ZuneTagContainerTestHelpers.SomeGuid);

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
            var container = ZuneTagContainerTestHelpers.CreateEmptyContainer();

            IEnumerable<MediaIdGuid> ids = container.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Then_it_should_be_able_to_add_a_mediaId_to_the_container()
        {
            var container = ZuneTagContainerTestHelpers.CreateEmptyContainer();

            var mediaIdGuid = new MediaIdGuid(MediaIds.ZuneMediaID, ZuneTagContainerTestHelpers.SomeGuid);

            container.Add(mediaIdGuid);

            var track = container.GetContainer().OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneMediaID).First();

            Assert.That(track.Owner, Is.EqualTo("ZuneMediaID"));
            Assert.That(new Guid(track.Data), Is.EqualTo(ZuneTagContainerTestHelpers.SomeGuid));
        }
    }

    [TestFixture]
    public class WhenATagContainerIsLoadedWithOnlyOneMediaIdButItIsIncorrect
    {
        [Test]
        public void Then_it_should_be_able_to_update_the_media_id_with_the_correct_guid()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithZuneAlbumartistMediaIDWithRandomGuid();

            var albumArtistMediaIdGuid = new MediaIdGuid(MediaIds.ZuneAlbumArtistMediaID,
                                                         ZuneTagContainerTestHelpers.SomeGuid);

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
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumArtist, Is.EqualTo("Various Artists"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_contributing_artist()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.ContributingArtist, Is.EqualTo(ZuneTagContainerTestHelpers.SomeArtist));
        }


        [Test]
        public void Then_it_should_be_able_to_get_the_album_title()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumTitle, Is.EqualTo(ZuneTagContainerTestHelpers.SomeAlbum));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_track_title()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.SongTitle, Is.EqualTo(ZuneTagContainerTestHelpers.SomeTitle));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_release_year()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.Year, Is.EqualTo(ZuneTagContainerTestHelpers.SomeYear));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_track_number()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.TrackNumber, Is.EqualTo("2"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_disc_number()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.DiscNumber, Is.EqualTo("2/2"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_genre()
        {
            var container = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.Genre, Is.EqualTo("Pop"));
        }
    }

    [TestFixture]
    public class WhenATagContainerIsLoadedWithNoMetaData
    {
        [Test]
        public void Then_it_should_return_blank_values()
        {
            ZuneTagContainer zuneTagContainer = ZuneTagContainerTestHelpers.CreateContainerWithNoMetaData();

            MetaData data = zuneTagContainer.ReadMetaData();

            Assert.That(data.Year, Is.EqualTo(""));
            Assert.That(data.TrackNumber, Is.EqualTo(""));
            Assert.That(data.Picture, Is.EqualTo(null));
        }
    }

    [TestFixture]
    public class WhenWritingMetaDataBackToFileWithNoMetaData
    {

        [Test]
        public void Then_it_should_be_able_to_write_all_the_meta_data()
        {
            MetaData metaData = new MetaData
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

            ZuneTagContainer zuneTagContainer = ZuneTagContainerTestHelpers.CreateContainerWithNoMetaData();

            zuneTagContainer.WriteMetaData(metaData);

            Assert.That(zuneTagContainer.ReadMetaData().AlbumArtist,Is.EqualTo(metaData.AlbumArtist));
            Assert.That(zuneTagContainer.ReadMetaData().ContributingArtist, Is.EqualTo(metaData.ContributingArtist));
            Assert.That(zuneTagContainer.ReadMetaData().AlbumTitle, Is.EqualTo(metaData.AlbumTitle));
            Assert.That(zuneTagContainer.ReadMetaData().DiscNumber, Is.EqualTo(metaData.DiscNumber));
            Assert.That(zuneTagContainer.ReadMetaData().Genre, Is.EqualTo(metaData.Genre));
            Assert.That(zuneTagContainer.ReadMetaData().SongTitle, Is.EqualTo(metaData.SongTitle));
            Assert.That(zuneTagContainer.ReadMetaData().TrackNumber, Is.EqualTo(metaData.TrackNumber));
            Assert.That(zuneTagContainer.ReadMetaData().Year, Is.EqualTo(metaData.Year));
        }
    }

    [TestFixture]
    public class WhenWritingMetaDataBackToFileWithPreExistingMetaData
    {
        [Test]
        public void Then_it_should_update_any_existing_metadata()
        {
            MetaData metaData = new MetaData
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

            ZuneTagContainer zuneTagContainer = ZuneTagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            zuneTagContainer.WriteMetaData(metaData);

            Assert.That(zuneTagContainer.ReadMetaData().AlbumArtist, Is.EqualTo(metaData.AlbumArtist));
            Assert.That(zuneTagContainer.ReadMetaData().ContributingArtist, Is.EqualTo(metaData.ContributingArtist));
            Assert.That(zuneTagContainer.ReadMetaData().AlbumTitle, Is.EqualTo(metaData.AlbumTitle));
            Assert.That(zuneTagContainer.ReadMetaData().DiscNumber, Is.EqualTo(metaData.DiscNumber));
            Assert.That(zuneTagContainer.ReadMetaData().Genre, Is.EqualTo(metaData.Genre));
            Assert.That(zuneTagContainer.ReadMetaData().SongTitle, Is.EqualTo(metaData.SongTitle));
            Assert.That(zuneTagContainer.ReadMetaData().TrackNumber, Is.EqualTo(metaData.TrackNumber));
            Assert.That(zuneTagContainer.ReadMetaData().Year, Is.EqualTo(metaData.Year));
        }
    }
}
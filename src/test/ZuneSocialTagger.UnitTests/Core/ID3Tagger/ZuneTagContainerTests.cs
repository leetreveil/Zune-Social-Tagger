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
        public void Then_it_should_read_a_list_of_three_media_ids()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithThreeZuneTags();

            IEnumerable<MediaIdGuid> ids = container.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumArtistMediaId()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithThreeZuneTags();

            MediaIdGuid result = container.ReadMediaIds().Where(x => x.MediaId == MediaIds.ZuneAlbumArtistMediaID).First();

            Assert.That(result.MediaId, Is.EqualTo(MediaIds.ZuneAlbumArtistMediaID));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumMediaId()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithThreeZuneTags();
            string mediaId = MediaIds.ZuneAlbumArtistMediaID;

            MediaIdGuid result = container.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumAMediaId()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithThreeZuneTags();
            string mediaId = MediaIds.ZuneMediaID;

            MediaIdGuid result = container.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_not_write_anything_to_the_tag_container()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithThreeZuneTags();
            var albumArtistMediaIdGuid = new MediaIdGuid
                                              {
                                                  Guid = ZuneTagContainerFactory.SomeGuid,
                                                  MediaId = MediaIds.ZuneAlbumArtistMediaID
                                              };
            var albumMediaIdGuid = new MediaIdGuid
                                        {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIDGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIDGuid};

            Assert.That(container.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(0));
        }
    }


    [TestFixture]
    public class WhenATagContainerIsLoadedWithNoMediaIdsPresent
    {
        [Test]
        public void Then_it_should_be_able_to_read_an_empty_list()
        {
            var container = ZuneTagContainerFactory.CreateEmptyContainer();

            IEnumerable<MediaIdGuid> ids = container.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Then_it_should_be_able_to_add_the_correct_ids_to_the_tag_container()
        {
            var container = ZuneTagContainerFactory.CreateEmptyContainer();

            //var zuneMediaIDWriter = new ZuneTagContainer(container);

            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            container.WriteMediaIdGuidsToContainer(guids);

            //check that the underlying container has actually got these values
            var artist = container.UnderlyingContainer.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumArtistMediaID).First();
            var album = container.UnderlyingContainer.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumMediaID).First();
            var track = container.UnderlyingContainer.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneMediaID).First();

            Assert.That(new Guid(artist.Data), Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
            Assert.That(new Guid(album.Data), Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
            Assert.That(new Guid(track.Data), Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_return_the_number_of_tags_added()
        {
            var container = ZuneTagContainerFactory.CreateEmptyContainer();

            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            Assert.That(container.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(3));
        }
    }


    [TestFixture]
    public class WhenATagContainerIsLoadedWithOnlyOneMediaIdButItIsIncorrect
    {
        [Test]
        public void Then_it_should_update_the_media_id_with_the_correct_guid()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithOneZuneTagWhichIsRandom();
      
            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            container.WriteMediaIdGuidsToContainer(guids);

            var artist = container.UnderlyingContainer.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumArtistMediaID).First();
            Assert.That(new Guid(artist.Data), Is.EqualTo(albumArtistMediaIdGuid.Guid));
        }

        [Test]
        public void Then_it_should_return_3_because_1_has_been_updated_and_2_have_been_added()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithOneZuneTagWhichIsRandom();

            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            Assert.That(container.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(3));
        }
    }

    [TestFixture]
    public class WhenATagContainerContainsMetaDataAboutTheTrack
    {
        [Test]
        public void Then_it_should_be_able_to_get_the_album_artist()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumArtist,Is.EqualTo(ZuneTagContainerFactory.SomeArtist));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_album_title()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumTitle, Is.EqualTo(ZuneTagContainerFactory.SomeAlbum));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_track_title()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.SongTitle, Is.EqualTo(ZuneTagContainerFactory.SomeTitle));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_release_year()
        {
            var container = ZuneTagContainerFactory.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.Year, Is.EqualTo(ZuneTagContainerFactory.SomeYear));
        }
    }
}
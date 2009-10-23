using System;
using System.Collections.Generic;
using System.Linq;
using ID3Tag.HighLevel;
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
            var reader = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());

            IEnumerable<MediaIdGuid> ids = reader.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumArtistMediaId()
        {
            var reader = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());

            MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == MediaIds.ZuneAlbumArtistMediaID).First();

            Assert.That(result.MediaId, Is.EqualTo(MediaIds.ZuneAlbumArtistMediaID));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumMediaId()
        {
            var reader = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());
            string mediaId = MediaIds.ZuneAlbumArtistMediaID;

            MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumAMediaId()
        {
            var reader = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());
            string mediaId = MediaIds.ZuneMediaID;

            MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_not_write_anything_to_the_tag_container()
        {
            var zuneMediaIDWriter = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());
            var _albumArtistMediaIdGuid = new MediaIdGuid
                                              {
                                                  Guid = ZuneTagContainerFactory.SomeGuid,
                                                  MediaId = MediaIds.ZuneAlbumArtistMediaID
                                              };
            var _albumMediaIdGuid = new MediaIdGuid
                                        {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var _mediaIDGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {_albumArtistMediaIdGuid, _albumMediaIdGuid, _mediaIDGuid};

            Assert.That(zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(0));
        }
    }


    [TestFixture]
    public class WhenATagContainerIsLoadedWithNoMediaIdsPresent
    {
        [Test]
        public void Then_it_should_be_able_to_read_an_empty_list()
        {
            var reader = new ZuneTagContainer(ZuneTagContainerFactory.CreateEmptyContainer());

            IEnumerable<MediaIdGuid> ids = reader.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Then_it_should_be_able_to_add_the_correct_ids_to_the_tag_container()
        {
            TagContainer container = ZuneTagContainerFactory.CreateEmptyContainer();

            var zuneMediaIDWriter = new ZuneTagContainer(container);

            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids);

            var artist = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumArtistMediaID).First();
            var album = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumMediaID).First();
            var track = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneMediaID).First();

            Assert.That(new Guid(artist.Data), Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
            Assert.That(new Guid(album.Data), Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
            Assert.That(new Guid(track.Data), Is.EqualTo(ZuneTagContainerFactory.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_return_the_number_of_tags_added()
        {
            var zuneMediaIDWriter = new ZuneTagContainer(ZuneTagContainerFactory.CreateEmptyContainer());

            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            Assert.That(zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(3));
        }
    }


    [TestFixture]
    public class WhenATagContainerIsLoadedWithOnlyOneMediaIdButItIsIncorrect
    {
        [Test]
        public void Then_it_should_update_the_media_id_with_the_correct_guid()
        {
            TagContainer container = ZuneTagContainerFactory.CreateContainerWithOneZuneTagWhichIsRandom();
            var zuneMediaIDWriter = new ZuneTagContainer(container);

            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids);

            var artist = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumArtistMediaID).First();
            Assert.That(new Guid(artist.Data), Is.EqualTo(albumArtistMediaIdGuid.Guid));
        }

        [Test]
        public void Then_it_should_return_3_because_1_has_been_updated_and_2_have_been_added()
        {
            var zuneMediaIDWriter =
                new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithOneZuneTagWhichIsRandom());

            var albumArtistMediaIdGuid = new MediaIdGuid
                                             {
                                                 Guid = ZuneTagContainerFactory.SomeGuid,
                                                 MediaId = MediaIds.ZuneAlbumArtistMediaID
                                             };
            var albumMediaIdGuid = new MediaIdGuid
                                       {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneAlbumMediaID};
            var mediaIdGuid = new MediaIdGuid {Guid = ZuneTagContainerFactory.SomeGuid, MediaId = MediaIds.ZuneMediaID};

            var guids = new List<MediaIdGuid> {albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid};

            Assert.That(zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(3));
        }
    }
}
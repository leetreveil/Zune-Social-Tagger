using System;
using System.Collections.Generic;
using System.Linq;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using NUnit.Framework;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    public class ZuneTagContainerTests
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
                string mediaId = "ZuneAlbumArtistMediaID";

                MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

                Assert.That(result.MediaId, Is.EqualTo(mediaId));
                Assert.That(result.Guid, Is.EqualTo(new Guid("3ed50a00-0600-11db-89ca-0019b92a3933")));
            }

            [Test]
            public void Then_it_should_read_the_ZuneAlbumMediaId()
            {
                var reader = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());
                string mediaId = "ZuneAlbumMediaID";

                MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

                Assert.That(result.MediaId, Is.EqualTo(mediaId));
                Assert.That(result.Guid, Is.EqualTo(new Guid("4f66ff01-0100-11db-89ca-0019b92a3933")));
            }

            [Test]
            public void Then_it_should_read_the_ZuneAlbumAMediaId()
            {
                var reader = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());
                string mediaId = "ZuneMediaID";

                MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

                Assert.That(result.MediaId, Is.EqualTo(mediaId));
                Assert.That(result.Guid, Is.EqualTo(new Guid("5366ff01-0100-11db-89ca-0019b92a3933")));
            }

            [Test]
            public void Then_it_should_not_write_anything_to_the_tag_container()
            {
                var zuneMediaIDWriter = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithThreeZuneTags());
                var _albumArtistMediaIdGuid = new MediaIdGuid { Guid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933"), MediaId = MediaIds.ZuneAlbumArtistMediaID };
                var _albumMediaIdGuid = new MediaIdGuid { Guid = new Guid("4f66ff01-0100-11db-89ca-0019b92a3933"), MediaId = MediaIds.ZuneAlbumMediaID };
                var _mediaIDGuid = new MediaIdGuid { Guid = new Guid("5366ff01-0100-11db-89ca-0019b92a3933"), MediaId = MediaIds.ZuneMediaID };

                var guids = new List<MediaIdGuid> { _albumArtistMediaIdGuid, _albumMediaIdGuid, _mediaIDGuid };

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

                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();
                var guid3 = Guid.NewGuid();

                var albumArtistMediaIdGuid = new MediaIdGuid { Guid = guid1, MediaId = MediaIds.ZuneAlbumArtistMediaID };
                var albumMediaIdGuid = new MediaIdGuid { Guid = guid2, MediaId = MediaIds.ZuneAlbumMediaID };
                var mediaIdGuid = new MediaIdGuid { Guid = guid3, MediaId = MediaIds.ZuneMediaID };

                var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

                zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids);

                var artist = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumArtistMediaID).First();
                var album = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumMediaID).First();
                var track = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneMediaID).First();

                Assert.That(new Guid(artist.Data), Is.EqualTo(guid1));
                Assert.That(new Guid(album.Data), Is.EqualTo(guid2));
                Assert.That(new Guid(track.Data), Is.EqualTo(guid3));
            }

            [Test]
            public void Then_it_should_be_able_to_return_the_number_of_tags_added()
            {
                var zuneMediaIDWriter = new ZuneTagContainer(ZuneTagContainerFactory.CreateEmptyContainer());

                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();
                var guid3 = Guid.NewGuid();

                var albumArtistMediaIdGuid = new MediaIdGuid { Guid = guid1, MediaId = MediaIds.ZuneAlbumArtistMediaID };
                var albumMediaIdGuid = new MediaIdGuid { Guid = guid2, MediaId = MediaIds.ZuneAlbumMediaID };
                var mediaIdGuid = new MediaIdGuid { Guid = guid3, MediaId = MediaIds.ZuneMediaID };

                var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

                Assert.That(zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(3));
            }

        }





        [TestFixture]
        public class WhenATagContainerIsLoadedWithOnlyOneMediaIdButItIsIncorrect
        {
            [Test]
            public void Then_it_should_update_the_media_id_with_the_correct_guid()
            {
                TagContainer container = ZuneTagContainerFactory.CreateContainerWithOneZuneTag();
                var zuneMediaIDWriter = new ZuneTagContainer(container);

                var albumArtistMediaIdGuid = new MediaIdGuid { Guid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumArtistMediaID" };
                var albumMediaIdGuid = new MediaIdGuid { Guid = new Guid("4f66ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumMediaID" };
                var mediaIdGuid = new MediaIdGuid { Guid = new Guid("5366ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneMediaID" };

                var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

                zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids);

                var artist = container.OfType<PrivateFrame>().Where(x => x.Owner == MediaIds.ZuneAlbumArtistMediaID).First();
                Assert.That(new Guid(artist.Data), Is.EqualTo(albumArtistMediaIdGuid.Guid));
            }

            [Test]
            public void Then_it_should_return_3_because_1_has_been_updated_and_2_have_been_added()
            {
                var zuneMediaIDWriter = new ZuneTagContainer(ZuneTagContainerFactory.CreateContainerWithOneZuneTag());

                var albumArtistMediaIdGuid = new MediaIdGuid { Guid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumArtistMediaID" };
                var albumMediaIdGuid = new MediaIdGuid { Guid = new Guid("4f66ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumMediaID" };
                var mediaIdGuid = new MediaIdGuid { Guid = new Guid("5366ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneMediaID" };

                var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

                Assert.That(zuneMediaIDWriter.WriteMediaIdGuidsToContainer(guids), Is.EqualTo(3));
            }


        }
    }
}
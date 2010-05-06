using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.IO.ID3Tagger;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenATagContainerIsLoadedWithTheCorrectMediaIdsPresent
    {
        [Test]
        public void Then_it_should_have_3_items()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            IEnumerable<ZuneAttribute> ids = container.ReadZuneAttributes();

            Assert.That(ids.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumArtistMediaId()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            ZuneAttribute result =
                container.ReadZuneAttributes().Where(x => x.Name == ZuneIds.Artist).First();

            Assert.That(result.Name, Is.EqualTo(ZuneIds.Artist));
            Assert.That(result.Guid, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumMediaId()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();
            string mediaId = ZuneIds.Artist;

            ZuneAttribute result = container.ReadZuneAttributes().Where(x => x.Name == mediaId).First();

            Assert.That(result.Name, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_ZuneAlbumAMediaId()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();
            string mediaId = ZuneIds.Track;

            ZuneAttribute result = container.ReadZuneAttributes().Where(x => x.Name == mediaId).First();

            Assert.That(result.Name, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_not_be_able_to_write_the_same_media_Id_to_the_container()
        {
            ZuneMP3TagContainer container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            var mediaIdGuid = new ZuneAttribute(ZuneIds.Track, ZuneMP3TagContainerTestHelpers.SomeGuid);

            container.AddZuneAttribute(mediaIdGuid);

            var mediaIds = container.ReadZuneAttributes();

            //we know that there are 3 items in the container so there should be no more
            Assert.That(mediaIds.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_be_able_to_remove_a_media_id_guid()
        {
            ZuneMP3TagContainer container = ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTags();

            container.RemoveZuneAttribute(ZuneIds.Track);

            var mediaIds = container.ReadZuneAttributes();

            Assert.That(mediaIds.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Then_it_should_be_able_to_remove_zune_attribute_when_there_are_more_than_one()
        {
            ZuneMP3TagContainer container =
                ZuneMP3TagContainerTestHelpers.CreateContainerWithThreeZuneTagsAndOneRepeating();

            container.RemoveZuneAttribute(ZuneIds.Track);
            container.RemoveZuneAttribute(ZuneIds.Artist);
            container.RemoveZuneAttribute(ZuneIds.Album);

            Assert.That(container.ReadZuneAttributes(),Is.Empty);
        }
    }


    [TestFixture]
    public class WhenATagContainerIsLoadedWithNoMediaIdsPresent
    {
        [Test]
        public void Then_the_read_media_ids_method_should_return_0()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateEmptyContainer();

            IEnumerable<ZuneAttribute> ids = container.ReadZuneAttributes();

            Assert.That(ids.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Then_it_should_be_able_to_add_a_mediaId_to_the_container()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateEmptyContainer();

            var mediaIdGuid = new ZuneAttribute(ZuneIds.Track, ZuneMP3TagContainerTestHelpers.SomeGuid);

            container.AddZuneAttribute(mediaIdGuid);

            var track = container.ReadZuneAttributes().Where(x=> x.Name == ZuneIds.Track).First();

            Assert.That(track.Guid, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeGuid));
        }

        [Test]
        public void Then_it_should_not_do_anything_when_trying_to_remove_a_frame()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateEmptyContainer();

            container.RemoveZuneAttribute(ZuneIds.Track);
        }

    }

    [TestFixture]
    public class WhenATagContainerIsLoadedWithOnlyOneMediaIdButItsValueIsIncorrect
    {
        [Test]
        public void Then_it_should_be_able_to_update_the_media_id_with_the_correct_guid()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithZuneAlbumartistMediaIDWithRandomGuid();

            //this guid does not equal the ZuneAlbumArtisMediaID Guid in the container
            var albumArtistMediaIdGuid = new ZuneAttribute(ZuneIds.Artist,
                                                         ZuneMP3TagContainerTestHelpers.SomeGuid);

            container.AddZuneAttribute(albumArtistMediaIdGuid);


            var artist = container.ReadZuneAttributes().Where(x => x.Name == ZuneIds.Artist).First();

            Assert.That(artist.Guid, Is.EqualTo(albumArtistMediaIdGuid.Guid));
        }
    }

    [TestFixture]
    public class WhenATagContainerContainsMetaDataAboutTheTrack
    {
        [Test]
        public void Then_it_should_be_able_to_get_the_album_artist()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumArtist, Is.EqualTo("Various Artists"));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_contributing_artist()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.ContributingArtists.First(), Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeArtist));
        }


        [Test]
        public void Then_it_should_be_able_to_get_the_album_title()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumName, Is.EqualTo(ZuneMP3TagContainerTestHelpers.SomeAlbum));
        }

        [Test]
        public void Then_it_should_be_able_to_get_the_track_title()
        {
            var container = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            MetaData metaData = container.ReadMetaData();

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

            Assert.That(metaData.TrackNumber, Is.EqualTo("2"));
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

            MetaData data = zuneMp3TagContainer.ReadMetaData();

            Assert.That(data.Year, Is.EqualTo(""));
            Assert.That(data.TrackNumber, Is.EqualTo(""));
        }
    }

    [TestFixture]
    public class WhenWritingMetaDataBackToContainerWithNoMetaData
    {

        [Test]
        public void Then_it_should_be_able_to_write_all_the_meta_data()
        {
            var metaData = new MetaData
                               {
                                        AlbumArtist = "Various Artists",
                                        AlbumName = "Forever",
                                        ContributingArtists = new List<string>{"U2","AFI"},
                                        DiscNumber = "1/1",
                                        Genre = "Pop",
                                        Title = "Wallet",
                                        TrackNumber = "2",
                                        Year = "2009"
                                 };

            ZuneMP3TagContainer zuneMp3TagContainer = ZuneMP3TagContainerTestHelpers.CreateContainerWithNoMetaData();

            zuneMp3TagContainer.AddMetaData(metaData);

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
            var metaData = new MetaData
            {
                AlbumArtist = "Various Artists",
                AlbumName = "Forever",
                ContributingArtists = new List<string> { "U2", "AFI" },
                DiscNumber = "1/1",
                Genre = "Pop",
                Title = "Wallet",
                TrackNumber = "2",
                Year = "2009"
            };

            ZuneMP3TagContainer zuneMp3TagContainer = ZuneMP3TagContainerTestHelpers.CreateContainerWithSomeStandardMetaData();

            zuneMp3TagContainer.AddMetaData(metaData);

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
using System;
using System.Collections.Generic;
using NUnit.Framework;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ID3Tagger;
using System.Linq;
using ASFTag.Net;

using ZuneSocialTagger.Core.WMATagger;

namespace ZuneSocialTagger.IntegrationTests.Core.WMATagger
{
    [TestFixture]
    public class WhenAWMAFileIsLoadedWithPreExistingZuneData
    {
        //we need to copy over the file instead of relying on the build to update it
        private string _path = "SampleData/asfheadercomplete.wma";

        [Test]
        public void Then_it_should_be_able_to_read_out_all_the_zune_data()
        {
            IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(_path);

            IEnumerable<MediaIdGuid> ids = container.ReadMediaIds().ToList();

            Assert.That(ids.Count(),Is.EqualTo(3));

            var mediaID = new MediaIdGuid(MediaIds.ZuneMediaID, new Guid("29c29901-0100-11db-89ca-0019b92a3933"));
            var albumArtistMediaID = new MediaIdGuid(MediaIds.ZuneAlbumArtistMediaID,
                                                     new Guid("760f0800-0600-11db-89ca-0019b92a3933"));

            var albumMediaID = new MediaIdGuid(MediaIds.ZuneAlbumMediaID,
                                               new Guid("25c29901-0100-11db-89ca-0019b92a3933"));

            Assert.That(ids.Contains(mediaID));
            Assert.That(ids.Contains(albumArtistMediaID));
            Assert.That(ids.Contains(albumMediaID));
        }

        [Test]
        public void Then_it_should_be_able_to_read_the_tracks_meta_data()
        {
            IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(_path);

            MetaData metaData = container.ReadMetaData();

            Assert.That(metaData.AlbumArtist,Is.EqualTo("The Decemberists"));
            Assert.That(metaData.AlbumName,Is.EqualTo("The Hazards of Love"));
            Assert.That(metaData.ContributingArtists, Is.EqualTo(new List<string> { "The Decemberists","Pendulum","AFI" }));
            Assert.That(metaData.DiscNumber,Is.EqualTo("1/1"));
            Assert.That(metaData.Genre,Is.EqualTo("Pop"));
            Assert.That(metaData.Title,Is.EqualTo("Prelude"));
            Assert.That(metaData.TrackNumber,Is.EqualTo("1"));
            Assert.That(metaData.Year,Is.EqualTo("2009"));
        }

        [Test]
        public void Then_it_should_be_able_to_update_the_zune_guids()
        {
            var container = new ZuneWMATagContainer(new TagContainer());

            Guid aGuid = Guid.NewGuid();
            container.AddZuneMediaId(new MediaIdGuid(MediaIds.ZuneAlbumArtistMediaID,aGuid));
            container.AddZuneMediaId(new MediaIdGuid(MediaIds.ZuneAlbumMediaID, aGuid));
            container.AddZuneMediaId(new MediaIdGuid(MediaIds.ZuneMediaID, aGuid));

            ASFTagManager.WriteTag(_path,container.GetContainer());

            TagContainer newContainer = ASFTagManager.ReadTag(_path);

            Assert.That(newContainer.Where(x=> x.Name == MediaIds.ZuneAlbumArtistMediaID).First().Value,Is.EqualTo(aGuid.ToString()));
            Assert.That(newContainer.Where(x => x.Name == MediaIds.ZuneAlbumMediaID).First().Value, Is.EqualTo(aGuid.ToString()));
            Assert.That(newContainer.Where(x => x.Name == MediaIds.ZuneMediaID).First().Value, Is.EqualTo(aGuid.ToString()));
        }

        [Test]
        public void Then_it_should_be_able_to_update_all_the_meta_data()
        {
            var container = new ZuneWMATagContainer(new TagContainer());

            var metaData = new MetaData
                {
                    AlbumArtist = "bleh",
                    AlbumName = "bleh",
                    ContributingArtists = new List<string> {"bleh", "bleh1", "bleh2"},
                    DiscNumber = "1",
                    Genre = "Pop",
                    Title = "YouTwo",
                    TrackNumber = "3",
                    Year = "2009"
                };

            container.AddMetaData(metaData);

            ASFTagManager.WriteTag(_path,container.GetContainer());

            IZuneTagContainer newContainer = ZuneTagContainerFactory.GetContainer(_path);

            MetaData newMetaData = newContainer.ReadMetaData();


            Assert.That(newMetaData.AlbumArtist,Is.EqualTo(metaData.AlbumArtist));
            Assert.That(newMetaData.AlbumName, Is.EqualTo(metaData.AlbumName));
            Assert.That(newMetaData.ContributingArtists.First(), Is.EqualTo(metaData.ContributingArtists.First()));
            Assert.That(newMetaData.ContributingArtists.ElementAt(1), Is.EqualTo(metaData.ContributingArtists.ElementAt(1)));
            Assert.That(newMetaData.ContributingArtists.Last(), Is.EqualTo(metaData.ContributingArtists.Last()));
            Assert.That(newMetaData.DiscNumber, Is.EqualTo(metaData.DiscNumber));
            Assert.That(newMetaData.Genre, Is.EqualTo(metaData.Genre));
            Assert.That(newMetaData.Title, Is.EqualTo(metaData.Title));
            Assert.That(newMetaData.TrackNumber, Is.EqualTo(metaData.TrackNumber));
            Assert.That(newMetaData.Year, Is.EqualTo(metaData.Year));
        }



    }

    [TestFixture]
    public class WhenAFileIsLoadedWithNoZuneMediaData
    {
        [Test]
        public void Then_it_should_be_able_to_add_all_3_ids()
        {
            Assert.Fail("not implemented");
        }

    }
}
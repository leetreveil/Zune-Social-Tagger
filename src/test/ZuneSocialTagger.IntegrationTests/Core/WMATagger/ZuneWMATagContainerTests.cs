using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.IO.WMATagger;

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

            IEnumerable<ZuneAttribute> ids = container.ReadZuneAttributes().ToList();

            Assert.That(ids.Count(),Is.EqualTo(3));

            var mediaID = new ZuneAttribute(ZuneIds.Track, new Guid("29c29901-0100-11db-89ca-0019b92a3933"));
            var albumArtistMediaID = new ZuneAttribute(ZuneIds.Artist,
                                                     new Guid("760f0800-0600-11db-89ca-0019b92a3933"));

            var albumMediaID = new ZuneAttribute(ZuneIds.Album,
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
            var container = ZuneWMATagContainerTestsHelpers.CreateEmptyContainer();

            Guid aGuid = Guid.NewGuid();
            container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Artist,aGuid));
            container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Album, aGuid));
            container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Track, aGuid));

            container.WriteToFile(_path);

            var newContainer = ZuneTagContainerFactory.GetContainer(_path);

            var mediaIds = newContainer.ReadZuneAttributes();

            Assert.That(mediaIds.Where(x => x.Name == ZuneIds.Artist).First().Guid, Is.EqualTo(aGuid));
            Assert.That(mediaIds.Where(x => x.Name == ZuneIds.Album).First().Guid, Is.EqualTo(aGuid));
            Assert.That(mediaIds.Where(x => x.Name == ZuneIds.Track).First().Guid, Is.EqualTo(aGuid));
        }

        [Test]
        public void Then_it_should_be_able_to_update_a_zune_attribute_with_a_new_value_and_not_add_a_new_one()
        {
            //TODO: fix failing test when tests are ran before it, this is because the file state is different when the test is ran
            var container = ZuneTagContainerFactory.GetContainer(_path);

            var oldCount = container.ReadZuneAttributes().Count();

            container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Artist,Guid.NewGuid()));

            Assert.That(container.ReadZuneAttributes().Count(),Is.EqualTo(oldCount));
        }


        [Test]
        public void Then_it_should_be_able_to_update_all_the_meta_data()
        {
            var container = ZuneWMATagContainerTestsHelpers.CreateEmptyContainer();

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

            container.WriteToFile(_path);

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

        [Test]
        public void Then_it_should_be_able_to_remove_all_zune_media_ids()
        {
            var container = (ZuneWMATagContainer) ZuneTagContainerFactory.GetContainer(_path);

            container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Artist,Guid.NewGuid()));

            container.RemoveZuneAttribute(ZuneIds.Artist);
            container.RemoveZuneAttribute(ZuneIds.Album);
            container.RemoveZuneAttribute(ZuneIds.Track);

            container.WriteToFile(_path);

            IZuneTagContainer tagContainer = ZuneTagContainerFactory.GetContainer(_path);

            Assert.That(tagContainer.ReadZuneAttributes(),Is.Empty);
        }


    }
}
using System;
using System.Collections.Generic;
using NUnit.Framework;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ID3Tagger;
using System.Linq;

namespace ZuneSocialTagger.IntegrationTests.Core.WMATagger
{
    [TestFixture]
    public class WhenAWMAFileIsLoadedWithPreExistingZuneData
    {
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

        }

    }
}
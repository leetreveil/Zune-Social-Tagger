using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.Linq;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenAnAudioFileIsLoadedWithThePrivateFramesRequiredToWorkWithZuneAreAlreadyPresent
    {
        //I know this file contains : ZuneAlbumArtistMediaId,ZuneAlbumMediaId and ZuneMediaId
        private string _mp3File =
            "SampleData/Editors - In This Light And On This Evening/01 - In This Light And On This Evening.mp3";

        [Test]
        public void Then_it_should_read_a_list_three_media_ids()
        {
            var reader = new ZuneMediaIdReader(_mp3File);

            IEnumerable<MediaIdGuid> ids = reader.ReadMediaIds();
       
            Assert.That(ids.Count(),Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumArtistMediaId()
        {
            var reader = new ZuneMediaIdReader(_mp3File);
            string mediaId = "ZuneAlbumArtistMediaID";

            MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(new Guid("3ed50a00-0600-11db-89ca-0019b92a3933")));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumMediaId()
        {
            var reader = new ZuneMediaIdReader(_mp3File);
            string mediaId = "ZuneAlbumMediaID";

            MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(new Guid("4f66ff01-0100-11db-89ca-0019b92a3933")));
        }

        [Test]
        public void Then_it_should_read_the_ZuneAlbumAMediaId()
        {
            var reader = new ZuneMediaIdReader(_mp3File);
            string mediaId = "ZuneMediaID";

            MediaIdGuid result = reader.ReadMediaIds().Where(x => x.MediaId == mediaId).First();

            Assert.That(result.MediaId, Is.EqualTo(mediaId));
            Assert.That(result.Guid, Is.EqualTo(new Guid("5366ff01-0100-11db-89ca-0019b92a3933")));
        }
    }

    [TestFixture]
    public class WhenAnAudioFileIsLoadedWithNoZuneDetailsPresent
    {
        //I know this file contains no zune media id's
        private string _mp3File =
            "SampleData/Editors - In This Light And On This Evening/02 - Bricks And Mortar.mp3";

        [Test]
        public void Then_it_should_get_an_empty_list_of_media_ids()
        {
            var reader = new ZuneMediaIdReader(_mp3File);

            IEnumerable<MediaIdGuid> ids = reader.ReadMediaIds();

            Assert.That(ids.Count(), Is.EqualTo(0));
        }
   
    }
}
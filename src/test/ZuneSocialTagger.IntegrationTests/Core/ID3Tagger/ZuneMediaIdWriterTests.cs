using System.Collections;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ID3Tagger;
using System;

namespace ZuneSocialTagger.IntegrationTests.Core.ID3Tagger
{
    [TestFixture]
    public class WhenAnAudioFileIsLoadedWithNoMediaIdsPresent
    {
        //I know this file contains no zune media id's
        private string _mp3File =
            "SampleData/Editors - In This Light And On This Evening/02 - Bricks And Mortar.mp3";

        [Test]
        public void Then_it_should_write_the_correct_ids_to_the_audio_file()
        {
            var zuneMediaIDReader = new ZuneMediaIdReader(_mp3File);
            var zuneMediaIDWriter = new ZuneMediaIdWriter(_mp3File,zuneMediaIDReader);

            var albumArtistMediaIdGuid = new MediaIdGuid {Guid = Guid.NewGuid(), MediaId = "ZuneAlbumArtistMediaID"};
            var albumMediaIdGuid = new MediaIdGuid {Guid = Guid.NewGuid(), MediaId = "ZuneAlbumMediaID"};
            var mediaIdGuid = new MediaIdGuid {Guid = Guid.NewGuid(), MediaId = "ZuneMediaID"};


            var guids = new List<MediaIdGuid>{albumArtistMediaIdGuid,albumMediaIdGuid,mediaIdGuid};

            zuneMediaIDWriter.WriteMediaIdGuids(guids);

            //TODO: we are not testing that the actual data is being writting and are just counting
            Assert.That(zuneMediaIDReader.ReadMediaIds().Count(),Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_return_the_correct_number_of_ids_written_to_file()
        {
            var zuneMediaIDWriter = new ZuneMediaIdWriter(_mp3File, new ZuneMediaIdReader(_mp3File));

            var albumArtistMediaIdGuid = new MediaIdGuid { Guid = Guid.NewGuid(), MediaId = "ZuneAlbumArtistMediaID" };
            var albumMediaIdGuid = new MediaIdGuid { Guid = Guid.NewGuid(), MediaId = "ZuneAlbumMediaID" };
            var mediaIdGuid = new MediaIdGuid { Guid = Guid.NewGuid(), MediaId = "ZuneMediaID" };

            var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

            Assert.That(zuneMediaIDWriter.WriteMediaIdGuids(guids), Is.EqualTo(3));
        }

        [Test]
        public void Then_it_should_write_the_correct_guids()
        {
            var reader = new ZuneMediaIdReader(_mp3File);
            var zuneMediaIDWriter = new ZuneMediaIdWriter(_mp3File, reader);

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            var albumArtistMediaIdGuid = new MediaIdGuid { Guid = guid1, MediaId = "ZuneAlbumArtistMediaID" };
            var albumMediaIdGuid = new MediaIdGuid { Guid = guid2, MediaId = "ZuneAlbumMediaID" };
            var mediaIdGuid = new MediaIdGuid { Guid = guid3, MediaId = "ZuneMediaID" };

            var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

            zuneMediaIDWriter.WriteMediaIdGuids(guids);

            var outGuids = reader.ReadMediaIds();

            var outArtistGuid = outGuids.Where(x => x.MediaId == "ZuneAlbumArtistMediaID").First();
            var outAlbumGuid = outGuids.Where(x => x.MediaId == "ZuneAlbumMediaID").First();
            var outMediaGuid = outGuids.Where(x => x.MediaId == "ZuneMediaID").First();

            Assert.That(outArtistGuid.Guid,Is.EqualTo(guid1));
            Assert.That(outAlbumGuid.Guid, Is.EqualTo(guid2));
            Assert.That(outMediaGuid.Guid, Is.EqualTo(guid3));
        }

    }

    public class WhenAnAudioFileIsLoadedWithTheCorrectMediaIdsPresent
    {
        private string _mp3File =
            "SampleData/Editors - In This Light And On This Evening/01 - In This Light And On This Evening.mp3";

        [Test]
        public void Then_it_should_not_write_anything_to_file_and_return_0()
        {
            //TODO: mock out the reader
            var zuneMediaIDWriter = new ZuneMediaIdWriter(_mp3File,new ZuneMediaIdReader(_mp3File));

            var albumArtistMediaIdGuid = new MediaIdGuid { Guid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumArtistMediaID" };
            var albumMediaIdGuid = new MediaIdGuid { Guid = new Guid("4f66ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumMediaID" };
            var mediaIdGuid = new MediaIdGuid { Guid = new Guid("5366ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneMediaID" };

            var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

            Assert.That(zuneMediaIDWriter.WriteMediaIdGuids(guids),Is.EqualTo(0));
        }

    }

    public class WhenAnAudioFileIsLoadedWithOnlyOneMediaIdButItIsIncorrect
    {
        private string _mp3File =
             "SampleData/Editors - In This Light And On This Evening/onemediaidthatisincorrect.mp3";

        [Test]
        public void Then_it_should_update_the_media_id_with_the_correct_guid()
        {
            var reader = new ZuneMediaIdReader(_mp3File);
            var zuneMediaIDWriter = new ZuneMediaIdWriter(_mp3File, reader);

            var albumArtistMediaIdGuid = new MediaIdGuid { Guid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumArtistMediaID" };
            var albumMediaIdGuid = new MediaIdGuid { Guid = new Guid("4f66ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumMediaID" };
            var mediaIdGuid = new MediaIdGuid { Guid = new Guid("5366ff01-0100-11db-89ca-0019b92a3933"),MediaId = "ZuneMediaID"};

            var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid };

            zuneMediaIDWriter.WriteMediaIdGuids(guids);

            MediaIdGuid condition = reader.ReadMediaIds().Where(x => x.MediaId == "ZuneAlbumArtistMediaID").First();
            Assert.That(condition.Guid,Is.EqualTo(albumArtistMediaIdGuid.Guid));
        }

        //TODO: we need to build before each test is run!!, need to reset the file back to default after each test
        //TODO: or maybe dont even write the guids to file and return a TagContainer which we check for validity

        [Test]
        public void Then_it_should_return_3()
        {
            var reader = new ZuneMediaIdReader(_mp3File);
            var zuneMediaIDWriter = new ZuneMediaIdWriter(_mp3File, reader);

            var albumArtistMediaIdGuid = new MediaIdGuid { Guid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumArtistMediaID" };
            var albumMediaIdGuid = new MediaIdGuid { Guid = new Guid("4f66ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneAlbumMediaID" };
            var mediaIdGuid = new MediaIdGuid { Guid = new Guid("5366ff01-0100-11db-89ca-0019b92a3933"), MediaId = "ZuneMediaID" };

            var guids = new List<MediaIdGuid> { albumArtistMediaIdGuid, albumMediaIdGuid, mediaIdGuid }; 

            Assert.That(zuneMediaIDWriter.WriteMediaIdGuids(guids), Is.EqualTo(3));
        }


    }
}
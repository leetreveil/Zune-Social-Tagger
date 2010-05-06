using System;
using Id3Tag;
using Id3Tag.HighLevel;
using Id3Tag.HighLevel.Id3Frame;
using System.Text;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.IO.ID3Tagger;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    public static class ZuneMP3TagContainerTestHelpers
    {
        /// <summary>
        /// 3ed50a00-0600-11db-89ca-0019b92a3933
        /// </summary>
        public static Guid SomeGuid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933");
        public static string SomeArtist = "Editors";
        public static string SomeAlbum = "In This Light And On This Evening";
        public static string SomeTitle = "The Boxer";
        public static string SomeYear = "2009";

        /// <summary>
        /// ZuneAlbumArtistMediaID: 3ed50a00-0600-11db-89ca-0019b92a3933
        /// ZuneAlbumMediaID: 4f66ff01-0100-11db-89ca-0019b92a3933
        /// ZuneMediaID: 5366ff01-0100-11db-89ca-0019b92a3933
        /// </summary>
        public static ZuneMP3TagContainer CreateContainerWithThreeZuneTags()
        {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame(ZuneIds.Artist, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Album, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Track, SomeGuid.ToByteArray()));

            return new ZuneMP3TagContainer(container);
        }

        /// <summary>
        /// ZuneAlbumArtistMediaID: 3ed50a00-0600-11db-89ca-0019b92a3933
        /// ZuneAlbumMediaID: 4f66ff01-0100-11db-89ca-0019b92a3933
        /// ZuneMediaID: 5366ff01-0100-11db-89ca-0019b92a3933
        /// </summary>
        public static ZuneMP3TagContainer CreateContainerWithThreeZuneTagsAndOneRepeating()
        {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame(ZuneIds.Artist, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Album, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Track, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(ZuneIds.Track, SomeGuid.ToByteArray()));

            return new ZuneMP3TagContainer(container);
        }

        public static ZuneMP3TagContainer CreateEmptyContainer()
        {
            return new ZuneMP3TagContainer(Id3TagFactory.CreateId3Tag(TagVersion.Id3V23));
        }

        /// <summary>
        /// Note: this contains an random guid to test for incorrectness
        /// </summary>
        /// <returns></returns>
        public static ZuneMP3TagContainer CreateContainerWithZuneAlbumartistMediaIDWithRandomGuid()
        {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame("ZuneAlbumArtistMediaID",
                                           Guid.NewGuid().ToByteArray()));

            return new ZuneMP3TagContainer(container);
        }

        /// <summary>
        /// Contains: AlbumArtist = "Various Artists", Artist = "Editors", Album= "In This Light And On This Evening", Title = "The Boxer", Year = "2009"
        /// </summary>
        /// <returns></returns>
        public static ZuneMP3TagContainer CreateContainerWithSomeStandardMetaData()
        {
            var container = Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new TextFrame("TALB", SomeAlbum, Encoding.UTF8));
            container.Add(new TextFrame("TPE1", SomeArtist, Encoding.UTF8));
            container.Add(new TextFrame("TPE2", "Various Artists", Encoding.UTF8));
            container.Add(new TextFrame("TIT2", SomeTitle, Encoding.UTF8));
            container.Add(new TextFrame("TYER", SomeYear, Encoding.UTF8));
            container.Add(new TextFrame("TRCK", "2", Encoding.UTF8));
            container.Add(new TextFrame("TPOS", "2/2", Encoding.UTF8));
            container.Add(new TextFrame("TCON", "Pop",Encoding.UTF8));

            return new ZuneMP3TagContainer(container);
        }

        public static ZuneMP3TagContainer CreateContainerWithNoMetaData()
        {
            return new ZuneMP3TagContainer(Id3TagFactory.CreateId3Tag(TagVersion.Id3V23));
        }
    }
}
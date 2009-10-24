using System;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using ZuneSocialTagger.Core.ID3Tagger;
using System.Text;

namespace ZuneSocialTagger.UnitTests.Core.ID3Tagger
{
    public static class ZuneTagContainerFactory
    {
        /// <summary>
        /// 3ed50a00-0600-11db-89ca-0019b92a3933
        /// </summary>
        public static Guid SomeGuid = new Guid("3ed50a00-0600-11db-89ca-0019b92a3933");

        /// <summary>
        /// ZuneAlbumArtistMediaID: 3ed50a00-0600-11db-89ca-0019b92a3933
        /// ZuneAlbumMediaID: 4f66ff01-0100-11db-89ca-0019b92a3933
        /// ZuneMediaID: 5366ff01-0100-11db-89ca-0019b92a3933
        /// </summary>
        public static ZuneTagContainer CreateContainerWithThreeZuneTags()
        {
            var container = ID3Tag.Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame(MediaIds.ZuneAlbumArtistMediaID, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(MediaIds.ZuneAlbumMediaID, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(MediaIds.ZuneMediaID, SomeGuid.ToByteArray()));

            return new ZuneTagContainer(container);
        }

        public static ZuneTagContainer CreateEmptyContainer()
        {
            return new ZuneTagContainer(ID3Tag.Id3TagFactory.CreateId3Tag(TagVersion.Id3V23));
        }

        /// <summary>
        /// Note: this contains an random guid to test for incorrectness
        /// </summary>
        /// <returns></returns>
        public static ZuneTagContainer CreateContainerWithZuneAlbumartistMediaIDWithRandomGuid()
        {
            var container = ID3Tag.Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame(MediaIds.ZuneAlbumArtistMediaID,
                                           Guid.NewGuid().ToByteArray()));

            return new ZuneTagContainer(container);
        }

        public static string SomeArtist = "Editors";
        public static string SomeAlbum = "In This Light And On This Evening";
        public static string SomeTitle = "The Boxer";
        public static string SomeYear = "2009";

        /// <summary>
        /// Contains: Artist = "Editors", Album= "In This Light And On This Evening", Title = "The Boxer", Year = "2009"
        /// </summary>
        /// <returns></returns>
        public static ZuneTagContainer CreateContainerWithSomeStandardMetaData()
        {
            var container = ID3Tag.Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new TextFrame("TALB", SomeAlbum, Encoding.UTF8));
            container.Add(new TextFrame("TPE1", SomeArtist, Encoding.UTF8));
            container.Add(new TextFrame("TIT2", SomeTitle, Encoding.UTF8));
            container.Add(new TextFrame("TYER", SomeYear, Encoding.UTF8));

            return new ZuneTagContainer(container);
        }
    }
}
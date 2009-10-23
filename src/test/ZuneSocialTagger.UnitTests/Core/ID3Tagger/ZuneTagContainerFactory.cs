using System;
using ID3Tag.HighLevel;
using ID3Tag.HighLevel.ID3Frame;
using ZuneSocialTagger.Core.ID3Tagger;

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
        public static TagContainer CreateContainerWithThreeZuneTags()
        {
            var container = ID3Tag.Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame(MediaIds.ZuneAlbumArtistMediaID, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(MediaIds.ZuneAlbumMediaID, SomeGuid.ToByteArray()));
            container.Add(new PrivateFrame(MediaIds.ZuneMediaID, SomeGuid.ToByteArray()));

            return container;
        }

        public static TagContainer CreateEmptyContainer()
        {
            return ID3Tag.Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);
        }

        /// <summary>
        /// Note: this contains an random guid to test for incorrectness
        /// </summary>
        /// <returns></returns>
        public static TagContainer CreateContainerWithOneZuneTagWhichIsRandom()
        {
            var container = ID3Tag.Id3TagFactory.CreateId3Tag(TagVersion.Id3V23);

            container.Add(new PrivateFrame(MediaIds.ZuneAlbumArtistMediaID,
                                           Guid.NewGuid().ToByteArray()));

            return container;
        }
    }
}
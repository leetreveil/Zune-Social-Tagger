using System.Linq;
using ZuneSocialTagger.Core.ZuneDatabase;

namespace ZuneSocialTagger.GUIV2.Models
{
    public static class ZuneTypeConverters
    {
        public static LinkStatus GetAlbumLinkStatus(string albumTitle, string albumArtist, Album existingAlbum)
        {
            bool artistTitlesMatch = albumArtist.ToUpper() ==
                                     existingAlbum.AlbumArtist.ToUpper();

            bool albumTitlesMatch = albumTitle.ToUpper() ==
                                    existingAlbum.AlbumTitle.ToUpper();


            //if first or second character of album title is different...
            //TODO: find better way to do comparison
            string firstTwoChars =
                new string(existingAlbum.AlbumTitle.ToUpper().Take(2).ToArray());

            bool albumTitlesFirstTwoCharactersMatch =
                albumTitle.ToUpper().StartsWith(firstTwoChars);

            if (!albumTitlesFirstTwoCharactersMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (!artistTitlesMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (albumTitlesMatch)
                return LinkStatus.Linked;


            return LinkStatus.Linked;
        }
    }
}
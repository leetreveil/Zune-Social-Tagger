using System;
using System.Linq;

namespace ZuneSocialTagger.GUI.Models
{
    public static class SharedMethods
    {
        public static LinkStatus GetAlbumLinkStatus(string albumTitle, string albumArtist, 
                                                    string existingAlbumTitle, string existingAlbumArtist)
        {
            bool artistTitlesMatch = albumArtist.ToUpper() ==
                                     existingAlbumArtist.ToUpper();

            bool albumTitlesMatch = albumTitle.ToUpper() ==
                                    existingAlbumTitle.ToUpper();


            //if first or second character of album title is different...
            //TODO: find better way to do comparison
            string firstTwoChars =
                new string(existingAlbumTitle.ToUpper().Take(2).ToArray());

            bool albumTitlesFirstTwoCharactersMatch = albumTitle.ToUpper().StartsWith(firstTwoChars);

            if (!albumTitlesFirstTwoCharactersMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (!artistTitlesMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (albumTitlesMatch)
                return LinkStatus.Linked;


           return LinkStatus.Linked;
        }

        public static bool DoesStringMatchWithoutTheAtStart(string first, string second)
        {
            if (first.ToUpper().StartsWith("THE"))
            {
                if (first.ToUpper().Skip(3) == second.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }

        public static Func<Song, int> SortByTrackNumber()
        {
            return key =>
            {
                int result;
                Int32.TryParse(key.MetaData.TrackNumber, out result);

                return result;
            };
        }

        public static LinkStatus GetLinkStatusFromGuid(this Guid guid)
        {
            return guid == Guid.Empty ? LinkStatus.Unlinked : LinkStatus.Unknown;
        }
    }
}
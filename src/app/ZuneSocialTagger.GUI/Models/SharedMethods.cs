using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZuneSocialTagger.GUI.Models
{
    public static class SharedMethods
    {
        public static LinkStatus GetAlbumLinkStatus(string albumTitle, string albumArtist, 
                                                    string existingAlbumTitle, string existingAlbumArtist)
        {
            string albumTitleCleaned = CleanString(albumTitle);
            string albumArtistCleaned = CleanString(albumArtist);
            string existingAlbumTitleCleaned = CleanString(existingAlbumTitle);
            string existingAlbumArtistCleaned = CleanString(existingAlbumArtist);

            bool artistTitlesMatch = albumArtistCleaned == existingAlbumArtistCleaned;
            bool albumTitlesMatch = albumTitleCleaned == existingAlbumTitleCleaned;


            //if first or second character of album title is different...
            //TODO: find better way to do comparison
            string firstTwoChars = new string(existingAlbumTitleCleaned.Take(2).ToArray());

            bool albumTitlesFirstTwoCharactersMatch = albumTitleCleaned.StartsWith(firstTwoChars);

            if (!albumTitlesFirstTwoCharactersMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (!artistTitlesMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (albumTitlesMatch)
                return LinkStatus.Linked;


           return LinkStatus.Linked;
        }

        private static string RemoveDiacritics(string stIn)
        {
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char t in
                from t in stFormD
                let uc = CharUnicodeInfo.GetUnicodeCategory(t)
                where uc != UnicodeCategory.NonSpacingMark
                select t)
            {
                sb.Append(t);
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }


        /// <summary>
        /// Takes a string and converts it to an easily comparable string, ToUpper + THE removes
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string CleanString(string input)
        {
            string result = input.ToUpper();

            if (result.StartsWith("THE "))
                result = new string(result.Skip(4).ToArray()); //skip THE and space

            if (result.StartsWith("A "))
                result = new string(result.Skip(2).ToArray());

            if (result.EndsWith(".") || result.EndsWith("?"))
                result = new string(result.Take(result.Count() - 1).ToArray());

            result = RemoveDiacritics(result);

            return result;
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
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.Models
{
    public static class SharedMethods
    {
        /// <summary>
        /// Converts 1 to 01 and 11 to 11 (the same)
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns></returns>
        public static string ConvertTrackNumberToDoubleDigits(this string trackNumber)
        {
            int number;
            if (!int.TryParse(trackNumber, out number))
            {
                throw new ArgumentException("number could not be converted");
            }

            //if we find a number with one digit then append a 0 to the start
            if (trackNumber.Length == 1)
            {
                return "0" + trackNumber;
            }

            return trackNumber;
        }

        public static ExpandedAlbumDetailsViewModel GetAlbumDetailsFrom(this DbAlbum album)
        {
            return new ExpandedAlbumDetailsViewModel
            {
                Artist = album.Artist,
                Title = album.Title,
                ArtworkUrl = album.ArtworkUrl,
                SongCount = album.TrackCount.ToString(),
                Year = album.ReleaseYear
            };
        }

        public static ExpandedAlbumDetailsViewModel GetAlbumDetailsFrom(this WebAlbum albumMetaData)
        {
            return new ExpandedAlbumDetailsViewModel
            {
                Title = albumMetaData.Title,
                Artist = albumMetaData.Artist,
                ArtworkUrl = albumMetaData.ArtworkUrl,
                Year = albumMetaData.ReleaseYear,
                SongCount = albumMetaData.Tracks.Count().ToString()
            };
        }


        /// <summary>
        /// Converts Track numbers that are like 1/2 to just 1 or 4/11 to just 4
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <returns></returns>
        public static string TrackNumberConverter(this string trackNumber)
        {
            if (String.IsNullOrEmpty(trackNumber)) return "0";

            return trackNumber.Contains('/') ? trackNumber.Split('/').First() : trackNumber;
        }

        /// <summary>
        /// Converts Disc number 1/2 to just one and 2/2 to just 2
        /// </summary>
        /// <param name="discNumber"></param>
        /// <returns></returns>
        public static string DiscNumberConverter(this string discNumber)
        {
            if (String.IsNullOrEmpty(discNumber)) return "1";

            return discNumber.Contains('/') ? discNumber.Split('/').First() : discNumber;
        }

        public static IZuneTagContainer GetContainer(this string filePath)
        {
            try
            {
                IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(filePath);
                return container;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<ErrorMessage,ApplicationViewModel>(new ErrorMessage(ErrorMode.Error, ex.Message));
                //if we hit an error on any track in the albums then just fail and dont read anymore
                return null;
            }
        }

        public static bool CheckIfZuneSoftwareIsRunning()
        {
            return Process.GetProcessesByName("Zune").Length != 0;
        }

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

        public static Func<Song, int> SortByTrackNumber()
        {
            return arg =>
            {
                int result;
                Int32.TryParse(arg.MetaData.TrackNumber, out result);

                return result;
            };
        }


        public static LinkStatus GetLinkStatusFromGuid(this Guid guid)
        {
            return guid == Guid.Empty ? LinkStatus.Unlinked : LinkStatus.Unknown;
        }
    }
}
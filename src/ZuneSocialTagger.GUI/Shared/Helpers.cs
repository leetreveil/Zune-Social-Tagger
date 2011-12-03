using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Models;
using System.Windows.Media.Imaging;
using System.IO;

namespace ZuneSocialTagger.GUI.Shared
{
    public static class Helpers
    {
        public static List<IZuneTagContainer> GetContainers(IEnumerable<string> filePaths)
        {
            var containers = new List<IZuneTagContainer>();

            foreach (var filePath in filePaths)
            {
                try
                {
                    var container = ZuneTagContainerFactory.GetContainer(filePath);
                    containers.Add(container);
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send(new ErrorMessage(ErrorMode.Error, ex.Message));
                }
            }

            return containers;
        }

        public static List<IZuneTagContainer> SortByTrackNumber(IList<IZuneTagContainer> containers)
        {
            var sorter = new Func<IZuneTagContainer, int>(arg => {
                int result;
                Int32.TryParse(arg.MetaData.TrackNumber, out result);
                return result;
            });

            return containers.OrderBy(sorter).ToList();
        }

        public static List<List<T>> Split<T>(this IEnumerable<T> source, int splitBy)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / splitBy)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }


        public static bool DoesAlbumTitleMatch(IEnumerable<string> albumTitlesToMatch, string albumTitleToMatch)
        {
            return albumTitlesToMatch.Any(albumTitle => albumTitle.ToLower().Contains(albumTitleToMatch.ToLower()));
        }

        public static bool DoesAlbumTitleMatchExactly(IEnumerable<string> albumTitlesToMatch, string albumTitleToMatch)
        {
            return albumTitlesToMatch.Any(albumTitle => albumTitle.ToLower() == albumTitleToMatch.ToLower());
        }

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
                return String.Empty;
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
            var eadvm = new ExpandedAlbumDetailsViewModel
            {
                Artist = album.Artist,
                Title = album.Title,
                SongCount = album.TrackCount.ToString(),
                Year = album.ReleaseYear
            };

            if (!String.IsNullOrEmpty(album.ArtworkUrl))
            {
                eadvm.Artwork = new BitmapImage(new Uri(album.ArtworkUrl));
            }

            return eadvm;
        }

        public static ExpandedAlbumDetailsViewModel GetAlbumDetailsFrom(this WebAlbum albumMetaData)
        {
            if (albumMetaData == null)
                return null;

            var eadvm =  new ExpandedAlbumDetailsViewModel
            {
                Title = albumMetaData.Title,
                Artist = albumMetaData.Artist,
                Year = albumMetaData.ReleaseYear,
                SongCount = albumMetaData.Tracks.Count().ToString()
            };

            if (!String.IsNullOrEmpty(albumMetaData.ArtworkUrl))
            {
                eadvm.Artwork = new BitmapImage(new Uri(albumMetaData.ArtworkUrl));
            }

            return eadvm;
        }

        public static ExpandedAlbumDetailsViewModel GetAlbumDetailsFrom(MetaData metaData, int trackCount)
        {
            if (metaData == null)
                return null;

            return new ExpandedAlbumDetailsViewModel
            {
                Artist = metaData.AlbumArtist,
                Title = metaData.AlbumName,
                SongCount = trackCount.ToString(),
                Year = metaData.Year
            };
        }

        public static ExpandedAlbumDetailsViewModel SetAlbumDetails(MetaData metaData, int trackCount)
        {
           var details = new  ExpandedAlbumDetailsViewModel
                   {
                       Artist = metaData.AlbumArtist,
                       Title = metaData.AlbumName,
                       SongCount = trackCount.ToString(),
                       Year = metaData.Year,
                   };

           if (metaData.Picture != null)
           {
               BitmapImage bi = new BitmapImage();
               bi.BeginInit();
               bi.StreamSource = new MemoryStream(metaData.Picture);
               bi.EndInit();
               details.Artwork = bi;
           }

            return details;
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

        public static bool CheckIfZuneSoftwareIsRunning()
        {
            return Process.GetProcessesByName("Zune").Length != 0;
        }

        public static LinkStatus GetLinkStatusFromGuid(this Guid guid)
        {
            return guid == Guid.Empty ? LinkStatus.Unlinked : LinkStatus.Unknown;
        }
    }
}
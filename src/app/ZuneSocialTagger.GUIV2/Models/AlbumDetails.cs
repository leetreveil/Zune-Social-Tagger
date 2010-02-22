using System;
using System.Windows.Media.Imaging;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumDetails
    {
        public AlbumDetails(int mediaId, string albumTitle, string albumArtist, string artworkUrl, string albumMediaId, DateTime dateAdded,int releaseYear, int trackCount)
        {
            MediaId = mediaId;
            AlbumMediaId = albumMediaId;
            ArtworkUrl = artworkUrl;
            AlbumTitle = albumTitle;
            AlbumArtist = albumArtist;
            DateAdded = dateAdded;
            TrackCount = trackCount;
            ReleaseYear = releaseYear;
        }

        public int MediaId { get; set; }
        public string AlbumMediaId { get; set; }
        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }
        public DateTime DateAdded { get; set; }
        public string ArtworkUrl { get; set; }
        public int TrackCount { get; set; }
        public int ReleaseYear { get; set; }

        public object ImageSource
        {
            get
            {
                var image = new BitmapImage();
                //TODO: find a better way instead of using goto :( to display blank artwork when an image cant be loaded

                //TODO: instead of goto; just set the image to the blank artwork
                try
                {
                    image.BeginInit();
                    image.UriSource = new Uri(this.ArtworkUrl, UriKind.RelativeOrAbsolute);
                    image.EndInit();
                }
                catch
                {
                    image.BeginInit();
                    const string uriToBlankArtwork = @"pack://application:,,,/Assets/blankartwork.png";

                    image.UriSource = new Uri(uriToBlankArtwork, UriKind.RelativeOrAbsolute);
                    this.ArtworkUrl = uriToBlankArtwork;
                    image.EndInit();
                }

                return image;
            }
        }
    }
}
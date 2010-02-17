using System;
using System.Windows.Media.Imaging;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumMetaData
    {
        private string _artworkUrl;

        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }

        public string ArtworkUrl
        {
            get { return _artworkUrl; }
            set
            {
                _artworkUrl = String.IsNullOrEmpty(value) ? @"pack://application:,,,/Assets/blankartwork.png" : value;
            }
        }

        public object ImageSource
        {
            get
            {
                BitmapImage image;

                //TODO: find a better way instead of using goto :( to display blank artwork when an image cant be loaded
            TryLabel:
                try
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnDemand;
                    image.CreateOptions = BitmapCreateOptions.None;
                    image.UriSource = new Uri(this.ArtworkUrl, UriKind.RelativeOrAbsolute);
                    image.EndInit();
                }
                catch
                {
                    this.ArtworkUrl = @"pack://application:,,,/Assets/blankartwork.png";

                    goto TryLabel;
                }

                return image;
            }
        }
    }
}
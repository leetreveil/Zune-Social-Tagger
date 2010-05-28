using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ZuneSocialTagger.GUI.Converters
{
    class PathToImageThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var artworkUrl = value as string;

            if (String.IsNullOrEmpty(artworkUrl)) return CreateBlankImage();

            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(artworkUrl, UriKind.RelativeOrAbsolute);
                image.EndInit();

                return image;
            }
            catch
            {
                return CreateBlankImage();
            }
        }

        private static BitmapImage CreateBlankImage()
        {
            var image = new BitmapImage();

			const string blankArtworkUrl =
                @"pack://application:,,,/ZuneSocialTagger.GUI;component/Resources/Assets/blankartwork.png";

            image.BeginInit();
            image.UriSource = new Uri(blankArtworkUrl, UriKind.RelativeOrAbsolute);
            image.EndInit();

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

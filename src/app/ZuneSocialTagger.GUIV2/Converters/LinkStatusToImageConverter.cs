using System;
using System.Globalization;
using System.Windows.Data;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.Converters
{
    public class LinkStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string urlToImageResource = string.Empty;

            switch ((LinkStatus) value)
            {
                case LinkStatus.Linked:
                    urlToImageResource = "pack://application:,,,/Assets/yes.png";
                    break;
                case LinkStatus.Unlinked:
                    break;
                case LinkStatus.AlbumOrArtistMismatch:
                    break;
                case LinkStatus.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }

            return urlToImageResource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.Converters
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
using System;
using System.Globalization;
using System.Windows.Data;
namespace ZuneSocialTagger.GUIV2.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class AlbumCountToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("ALBUMS ({0})", (int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
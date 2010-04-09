using System;
using System.Globalization;
using System.Windows.Data;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2.Converters
{
    public class ZuneMessageBoxButtonToVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (ZuneMessageBoxButton) value;

            switch (mode)
            {
                case ZuneMessageBoxButton.OK:
                    return false;
                case ZuneMessageBoxButton.OKCancel:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;


namespace ZuneSocialTagger.GUI.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class TextToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value != "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
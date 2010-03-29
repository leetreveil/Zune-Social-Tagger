using System;
using System.Globalization;
using System.Windows.Data;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Converters
{
    public class ErrorModeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errorMode = (ErrorMode) value;

            switch (errorMode)
            {
                case ErrorMode.Error:
                    return "../Assets/no.png";
                case ErrorMode.Warning:
                    return "../Assets/warning.png";
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
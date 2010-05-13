using System;
using System.Globalization;
using System.Windows.Data;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.Converters
{
    public class ErrorModeToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errorMode = (ErrorMode) value;

            switch (errorMode)
            {
                case ErrorMode.Error:
                    return "../Resources/Assets/no.png";
                case ErrorMode.Warning:
                    return "../Resources/Assets/warning.png";
                case ErrorMode.Info:
                    return "../Resources/Assets/information.png";
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
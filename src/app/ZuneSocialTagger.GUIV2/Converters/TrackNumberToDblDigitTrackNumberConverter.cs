using System;
using System.Globalization;
using System.Windows.Data;

namespace ZuneSocialTagger.GUIV2.Converters
{
    public class TrackNumberToDblDigitTrackNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (string) value;

            int number;
            if (!int.TryParse(input, out number))
            {
                //bum out if it isnt a real number
                return input;
            }

            //if we find a number with one digit then append a 0 to the start
            if (input.Length == 1)
            {
                return 0 + input;
            }

            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
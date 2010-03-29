using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.Converters
{
    public class SortOrderToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sortOrder = (SortOrder) value;

            return sortOrder == SortOrder.NotSorted ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
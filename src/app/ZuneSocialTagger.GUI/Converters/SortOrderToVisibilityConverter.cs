using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.Converters
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
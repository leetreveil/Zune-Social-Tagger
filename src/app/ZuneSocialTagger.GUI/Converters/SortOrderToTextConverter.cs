using System;
using System.Globalization;
using System.Windows.Data;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.Converters
{
    public class SortOrderToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sortOrder = (SortOrder) value;

            switch (sortOrder)
            {
                case SortOrder.DateAdded:
                    return "DATE ADDED";
                case SortOrder.Album:
                    return "ALBUM";
                case SortOrder.Artist:
                    return "ARTIST";
                case SortOrder.LinkStatus:
                    return "LINK STATUS";
                case SortOrder.NotSorted:
                    return String.Empty;
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
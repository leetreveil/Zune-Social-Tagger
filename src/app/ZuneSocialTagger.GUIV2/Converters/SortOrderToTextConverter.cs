using System;
using System.Globalization;
using System.Windows.Data;
using ZuneSocialTagger.GUIV2.ViewModels;


namespace ZuneSocialTagger.GUIV2.Converters
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
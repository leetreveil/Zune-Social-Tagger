using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ZuneSocialTagger.GUI.Converters
{
		public class TextToVisibilityConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
			    return String.IsNullOrEmpty((string) value) ? Visibility.Visible : Visibility.Collapsed;
			}

		    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return null;
			}
		}
}
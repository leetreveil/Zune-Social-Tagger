using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2
{
	/// <summary>
	/// Interaction logic for SearchBar.xaml
	/// </summary>
	public partial class SearchBar : UserControl
	{
		public SearchBar()
		{
			this.InitializeComponent();
		}

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO: change this because we should not be casting to SearchViewModel inside a sub view
            var dataContext = (SearchViewModel) this.DataContext;
            var tb = (TextBox) sender;

            dataContext.FlagCanMoveNext = tb.Text.Length > 0;
            dataContext.SearchText = tb.Text;
        }
	}
}
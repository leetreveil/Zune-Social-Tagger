using System.Windows.Controls;
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
            var tb = (TextBox) sender;

            var context = (SearchBarViewModel)this.DataContext;

            //dataContext.FlagCanMoveNext = tb.Text.Length > 0;
            context.SearchText = tb.Text;
        }
	}
}
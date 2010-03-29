using System.Windows.Controls;
using System.Windows.Input;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for SearchBarView.xaml
    /// </summary>
    public partial class SearchBarView : UserControl
    {
        public SearchBarView()
        {
            this.InitializeComponent();
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var searchBarView = (SearchBarViewModel) this.DataContext;

            if (e.Key == Key.Enter)
                searchBarView.Search();
        }
    }
}
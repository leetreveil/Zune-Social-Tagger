using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            this.InitializeComponent();
        }

        private void SearchBarControl_SearchClicked()
        {
            var searchViewModel = (SearchViewModel) this.DataContext;

            searchViewModel.Search();
        }
    }
}
using System.Windows.Controls;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Views
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
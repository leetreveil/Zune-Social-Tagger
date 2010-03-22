using System.Windows.Controls;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for SearchResultsView.xaml
    /// </summary>
    public partial class SearchResultsView : UserControl
    {
        public SearchResultsView()
        {
            this.InitializeComponent();
        }

        private void lvAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var searchResultsViewModel = (SearchResultsViewModel) this.DataContext;

            if (searchResultsViewModel != null)
            {
                var selectedItem = (Album)lvAlbums.SelectedItem;

                if (selectedItem != null)
                {
                    searchResultsViewModel.LoadAlbum(selectedItem);
                }
            }
        }
    }
}
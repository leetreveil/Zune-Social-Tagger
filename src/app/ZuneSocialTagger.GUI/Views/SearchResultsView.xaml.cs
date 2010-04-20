using System.Collections.Specialized;
using System.Windows.Controls;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Views
{
    /// <summary>
    /// Interaction logic for SearchResultsView.xaml
    /// </summary>
    public partial class SearchResultsView : UserControl
    {
        private SearchResultsViewModel _searchResultsViewModel;

        public SearchResultsView()
        {
            this.InitializeComponent();
            this.DataContextChanged += SearchResultsView_DataContextChanged;
        }

        void SearchResultsView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ( e.NewValue != null && e.NewValue.GetType() == typeof(SearchResultsViewModel))
            {
                _searchResultsViewModel = (SearchResultsViewModel) e.NewValue;

                if (_searchResultsViewModel.Albums == null) return;
                    _searchResultsViewModel.Albums.CollectionChanged += Albums_CollectionChanged;
            }
        }

        void Albums_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewStartingIndex == 0)
            {
                lvAlbums.SelectedIndex = 0;
            }
        }

        private void lvAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_searchResultsViewModel != null)
            {
                var selectedItem = (Album)lvAlbums.SelectedItem;

                if (selectedItem != null)
                {
                    _searchResultsViewModel.LoadAlbum(selectedItem);
                }
            }
        }
    }
}
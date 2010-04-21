using System.Collections.Specialized;
using System.Windows.Controls;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.ViewModels;
using System.Diagnostics;

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

                if (_searchResultsViewModel.SearchResults == null) return;
                    _searchResultsViewModel.SearchResults.CollectionChanged += Albums_CollectionChanged;
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
                if (lvAlbums.SelectedItem != null)
                {
                    if (lvAlbums.SelectedItem.GetType() == typeof(Album) && lvAlbums.SelectedIndex == 0)
                    {
                        _searchResultsViewModel.LoadAlbum((Album)lvAlbums.SelectedItem);
                    }
                }
            }
        }

        private void lvAlbums_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lvAlbums.SelectedItem != null)
            {
                if (lvAlbums.SelectedItem.GetType() == typeof (Artist))
                    _searchResultsViewModel.LoadAlbumsForArtist((Artist) lvAlbums.SelectedItem);

                if (lvAlbums.SelectedItem.GetType() == typeof (Album))
                    _searchResultsViewModel.LoadAlbum((Album) lvAlbums.SelectedItem);
            }
        }
    }
}
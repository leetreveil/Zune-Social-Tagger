using System;
using System.Windows.Controls;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Collections.Specialized;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for SearchResultsView.xaml
    /// </summary>
    public partial class SearchResultsView : UserControl
    {
        private SearchResultsViewModel _viewModel;

        public SearchResultsView()
        {
            this.InitializeComponent();
            this.DataContextChanged += SearchResultsView_DataContextChanged;
        }

        private void SearchResultsView_DataContextChanged(object sender,
                                                          System.Windows.DependencyPropertyChangedEventArgs e)
        {
            _viewModel = (SearchResultsViewModel) this.DataContext;

            if (_viewModel != null)
                _viewModel.SearchBarViewModel.SearchResults.CollectionChanged += SearchResults_CollectionChanged;
        }

        private void SearchResults_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.OldItems == null && e.NewItems.Count > 0)
                this.lvAlbums.SelectedIndex = 0;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && e.AddedItems.Count > 0)
            {
                var result = (Album) e.AddedItems[0];

                _viewModel.LoadAlbum(String.Concat(Urls.Album, result.AlbumMediaID.ToString()));
            }
        }
    }
}
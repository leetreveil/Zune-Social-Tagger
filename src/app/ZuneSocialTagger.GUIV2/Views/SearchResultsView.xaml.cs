using System;
using System.Windows.Controls;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.ViewModels;

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

        void SearchResultsView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            _viewModel = (SearchResultsViewModel)this.DataContext;

            if (_viewModel != null)
                _viewModel.SearchBarViewModel.FirstItemsFound += SearchBarViewModel_ItemsFound;
        }

        void SearchBarViewModel_ItemsFound(object sender, System.EventArgs e)
        {
            Console.WriteLine("found!");
            this.lvAlbums.Dispatcher.Invoke(new Action(() => this.lvAlbums.SelectedIndex = 0));
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && e.AddedItems.Count > 0)
            {
                _viewModel.LoadAlbum((AlbumSearchResult)e.AddedItems[0]);
            }
        }
    }
}
using System;
using System.Windows.Controls;
using ZuneSocialTagger.Core.ZuneWebsite;
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
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && e.AddedItems.Count > 0)
            {
                var result = (Album)e.AddedItems[0];

                _viewModel.LoadAlbum(String.Concat("http://catalog.zune.net/v3.0/en-US/music/album/",result.AlbumMediaID.ToString()));
            }
        }
    }
}
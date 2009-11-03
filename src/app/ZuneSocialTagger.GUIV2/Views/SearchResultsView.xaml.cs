using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Linq;

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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dContext = (SearchResultsViewModel) this.DataContext;

            if (dContext != null)
                dContext.LoadAlbum((AlbumArtistAndTitleWithUrl) e.AddedItems[0]);
        }

        private void StackPanel_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var dContext = (SearchResultsViewModel) this.DataContext;

            if (dContext != null)
                dContext.ViewShown();
        }

  
    }
}
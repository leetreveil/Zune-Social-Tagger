using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Linq;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for WebAlbumListView.xaml
    /// </summary>
    public partial class WebAlbumListView : UserControl
    {
        private WebAlbumListViewModel _model;

        public WebAlbumListView()
        {
            this.InitializeComponent();
			
            // Insert code required on object creation below this point.
            this.DataContextChanged += delegate { _model = (WebAlbumListViewModel) this.DataContext; };
        }

        private void LinkAlbum_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.Albums.Select(x=> lvAlbums.SelectedItem as AlbumDetailsViewModel).First().LinkAlbum();
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.Albums.Select(x => lvAlbums.SelectedItem as AlbumDetailsViewModel).First().RefreshAlbum();
        }

        private void DelinkAlbum_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.Albums.Select(x => lvAlbums.SelectedItem as AlbumDetailsViewModel).First().DelinkAlbum();
        }
    }
}
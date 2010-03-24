using System.Linq;
using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.ViewModels;

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

            _model = (WebAlbumListViewModel)this.DataContext;
        }

        private void LinkAlbum_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.LinkAlbum(GetSelectedAlbum().ZuneAlbumMetaData);
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.RefreshAlbum(GetSelectedAlbum());
        }

        private void DelinkAlbum_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.DelinkAlbum(GetSelectedAlbum().ZuneAlbumMetaData);
        }

        private AlbumDetailsViewModel GetSelectedAlbum()
        {
            return (AlbumDetailsViewModel)lvAlbums.SelectedItem;
        }

    }
}
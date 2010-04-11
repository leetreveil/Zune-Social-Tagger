using System.Windows;
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
            this.DataContextChanged += delegate { _model = (WebAlbumListViewModel) this.DataContext; };
            this.Loaded += delegate
                               {
                                   if (lvAlbums.SelectedItem != null)
                                   {
                                       lvAlbums.ScrollIntoView(lvAlbums.SelectedItem);
                                       var itemCont = (ListViewItem)lvAlbums.ItemContainerGenerator.ContainerFromItem(lvAlbums.SelectedItem);
                                       itemCont.Focus();
                                   }
                               };
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
using System.Windows.Controls;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Views
{
    /// <summary>
    /// Interaction logic for AlbumDetailsView.xaml
    /// </summary>
    public partial class AlbumDetailsView : UserControl
    {
        public AlbumDetailsView()
        {
            InitializeComponent();
        }

        private void LinkAlbum_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = this.DataContext as AlbumDetailsViewModel;
            if (model != null) model.LinkAlbum();
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = this.DataContext as AlbumDetailsViewModel;
            if (model != null) model.RefreshAlbum();
        }

        private void DelinkAlbum_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = this.DataContext as AlbumDetailsViewModel;
            if (model != null) model.DelinkAlbum();
        }

    }
}

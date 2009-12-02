using System.Windows.Controls;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for WebsiteAlbumMetaDataView.xaml
    /// </summary>
    public partial class WebsiteAlbumMetaDataView : UserControl
    {
        public WebsiteAlbumMetaDataView()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ArtworkImage_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RightClickMenu.PlacementTarget = this;
            RightClickMenu.IsOpen = true;
        }
    }
}
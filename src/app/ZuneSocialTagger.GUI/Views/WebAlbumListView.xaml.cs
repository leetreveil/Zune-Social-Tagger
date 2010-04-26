using System.Windows.Controls;

namespace ZuneSocialTagger.GUI.Views
{
    /// <summary>
    /// Interaction logic for WebAlbumListView.xaml
    /// </summary>
    public partial class WebAlbumListView : UserControl
    {
        public WebAlbumListView()
        {
            this.InitializeComponent();
            this.Loaded += WebAlbumListView_Loaded;
        }

        void WebAlbumListView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (lvAlbums.SelectedItem != null)
            {
                lvAlbums.ScrollIntoView(lvAlbums.SelectedItem);
                var item = (ListViewItem)lvAlbums.ItemContainerGenerator.
                    ContainerFromItem(lvAlbums.SelectedItem);
                item.Focus();
            }

            e.Handled = true;
        }
    }
}
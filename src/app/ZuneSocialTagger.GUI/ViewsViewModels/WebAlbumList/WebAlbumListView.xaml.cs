using System.Windows;
using System.Windows.Controls;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
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
            //hack to get the contextmenu to register for databinding
            NameScope.SetNameScope(contextMenu,NameScope.GetNameScope(this));
        }

        void WebAlbumListView_Loaded(object sender, RoutedEventArgs e)
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
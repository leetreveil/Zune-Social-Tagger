using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            // first load is a hack around the view not respecting the selected album in the view model before view load
            var dcon = (WebAlbumListViewModel)this.DataContext;
            if (lvAlbums.SelectedItem != null && !dcon.FirstLoad)
            {
                dcon.FirstLoad = false;
                lvAlbums.ScrollIntoView(lvAlbums.SelectedItem);
                var item = (ListViewItem)lvAlbums.ItemContainerGenerator.
                    ContainerFromItem(lvAlbums.SelectedItem);
                item.Focus();
            }
            else
            {
                lvAlbums.SelectedIndex = 0;
            }

            e.Handled = true;
        }
    }
}
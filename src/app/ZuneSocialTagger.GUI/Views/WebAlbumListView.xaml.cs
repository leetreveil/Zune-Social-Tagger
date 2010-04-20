using System.Windows.Controls;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Views
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

        private void SortView_SortClicked(Models.SortOrder sortOrder)
        {
            _model.SortData(sortOrder);
        }

    }
}
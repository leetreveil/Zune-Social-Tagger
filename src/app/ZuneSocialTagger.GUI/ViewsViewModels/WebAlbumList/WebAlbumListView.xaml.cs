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
            //hack to get the contextmenu to register for databinding
            NameScope.SetNameScope(contextMenu,NameScope.GetNameScope(this));
        }
    }
}
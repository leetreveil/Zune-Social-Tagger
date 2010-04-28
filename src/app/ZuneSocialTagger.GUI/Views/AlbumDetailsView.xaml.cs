using System.Windows.Controls;
using System.Windows;

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

            //hack to get the contextmenu to register for databinding
            NameScope.SetNameScope(contextMenu,NameScope.GetNameScope(this));
        }
    }
}

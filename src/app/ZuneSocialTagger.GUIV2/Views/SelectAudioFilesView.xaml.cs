using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for SelectAudioFilesView.xaml
    /// </summary>
    public partial class SelectAudioFilesView : UserControl
    {
        private SelectAudioFilesViewModel _model;
        public SelectAudioFilesView()
        {
            this.InitializeComponent();
			
            // Insert code required on object creation below this point.

            this.DataContextChanged += delegate { _model = (SelectAudioFilesViewModel) this.DataContext; };
        }

        private void GridViewColumnHeader_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GridViewColumnHeader header = (GridViewColumnHeader) e.OriginalSource;

            //header.Column.
           // _model.Sort((string) header.Column.Header);
        }

        private void LinkAlbum_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _model.LinkAlbum((Album) lvAlbums.SelectedItem);
        }
    }
}
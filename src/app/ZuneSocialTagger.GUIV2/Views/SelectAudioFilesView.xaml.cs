using System.Windows.Controls;
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
            var header = (GridViewColumnHeader) e.OriginalSource;

            _model.Sort((string) header.Column.Header);
        }

    }
}
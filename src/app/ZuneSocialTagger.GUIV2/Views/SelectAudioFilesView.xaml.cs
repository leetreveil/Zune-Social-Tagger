using System.Windows;
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
            InitializeComponent();
            this.DataContextChanged += delegate { _model = (SelectAudioFilesViewModel) this.DataContext; };
            this.Loaded += SelectAudioFilesView_Loaded;
        }

        void SelectAudioFilesView_Loaded(object sender, RoutedEventArgs e)
        {
            _model.ViewHasFinishedLoading();
        }
    }
}

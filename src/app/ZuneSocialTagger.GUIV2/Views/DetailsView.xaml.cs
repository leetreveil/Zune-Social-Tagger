using System.Windows;
using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for DetailsView.xaml
    /// </summary>
    public partial class DetailsView : UserControl
    {
        private DetailsViewModel _viewModel;

        public DetailsView()
        {
            InitializeComponent();

            this.DataContextChanged += DetailsView_DataContextChanged;
        }

        void DetailsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = (DetailsViewModel) this.DataContext;
        }
    }
}

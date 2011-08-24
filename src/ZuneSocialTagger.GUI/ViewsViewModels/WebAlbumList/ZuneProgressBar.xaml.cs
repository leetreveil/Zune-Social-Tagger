using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    /// <summary>
    /// Interaction logic for ZuneProgressBar.xaml
    /// </summary>
    public partial class ZuneProgressBar : ProgressBar
    {
        public ZuneProgressBar()
        {
            InitializeComponent();
            this.ValueChanged += ZuneProgressBar_ValueChanged;
        }

        void ZuneProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Visibility = e.NewValue == 0 ? Visibility.Collapsed : Visibility.Visible;

            //reset the progress bar to 0 once it gets to its maximum
            //and make it hidden
            if (e.NewValue == this.Maximum)
            {
                this.Value = 0;
            }
        }
    }
}

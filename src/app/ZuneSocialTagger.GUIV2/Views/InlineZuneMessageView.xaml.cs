using System.Windows;
using System.Windows.Controls;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for InlineZuneMessageView.xaml
    /// </summary>
    public partial class InlineZuneMessageView : UserControl
    {
        public InlineZuneMessageView()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.RootVisual.Visibility = Visibility.Collapsed;
        }
    }

}
using System.Windows;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : DraggableWindow
    {
        public AboutView()
        {
            InitializeComponent();
            this.DataContext = new AboutViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

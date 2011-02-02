using System.Windows;
using ZuneSocialTagger.GUI.Controls;

namespace ZuneSocialTagger.GUI.ViewsViewModels.About
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : DraggableWindow
    {
        public AboutView()
        {
            InitializeComponent();
            this.Owner =  System.Windows.Application.Current.MainWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

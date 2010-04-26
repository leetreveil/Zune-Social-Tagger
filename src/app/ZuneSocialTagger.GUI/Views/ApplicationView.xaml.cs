using System.Windows;
using ZuneSocialTagger.GUI.Controls;

namespace ZuneSocialTagger.GUI.Views
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : DraggableWindow
    {
        public ApplicationView()
        {
            InitializeComponent();
        }

        void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}

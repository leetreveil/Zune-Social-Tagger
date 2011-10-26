using System.Windows;
using System.Windows.Input;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Shared;
using GalaSoft.MvvmLight.Messaging;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
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

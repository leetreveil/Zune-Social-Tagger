using System.Windows;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Views
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : DraggableWindow
    {
        public UpdateView(UpdateViewModel updateViewModel)
        {
            InitializeComponent();

            this.DataContext = updateViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

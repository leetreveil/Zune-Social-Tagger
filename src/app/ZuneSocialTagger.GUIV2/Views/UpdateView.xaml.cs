using System.Windows;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
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

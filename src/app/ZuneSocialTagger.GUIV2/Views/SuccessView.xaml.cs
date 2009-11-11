using System.Windows;
using System.Windows.Input;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for SuccessView.xaml
    /// </summary>
    public partial class SuccessView : Window
    {
        public SuccessView(SuccessViewModel model)
        {
            InitializeComponent();

            this.DataContext = model;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

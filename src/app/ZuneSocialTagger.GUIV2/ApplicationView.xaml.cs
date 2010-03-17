using System.Windows;
using System.Windows.Input;

namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : Window
    {
        private readonly ApplicationModel _model;
        public ApplicationView()
        {
            InitializeComponent();

            Application.Current.Exit += Current_Exit;

           _model = (ApplicationModel) this.DataContext;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            _model.ShuttingDown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
           Application.Current.Shutdown();
        }
    }
}

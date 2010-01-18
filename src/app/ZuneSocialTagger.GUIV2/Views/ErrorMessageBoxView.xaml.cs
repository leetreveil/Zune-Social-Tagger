using System.Windows;
using System.Windows.Input;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for ErrorMessageBoxView.xaml
    /// </summary>
    public partial class ErrorMessageBoxView : Window
    {
        private readonly string _errorMessage;

        public ErrorMessageBoxView(string errorMessage)
        {
            InitializeComponent();

            _errorMessage = errorMessage;
            this.DataContext = this;
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for ErrorMessageBoxView.xaml
    /// </summary>
    public partial class ErrorMessageBoxView : Window
    {
        private readonly string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public ErrorMessageBoxView(string errorMessage)
        {
            InitializeComponent();

            _errorMessage = errorMessage;
            this.DataContext = this;
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

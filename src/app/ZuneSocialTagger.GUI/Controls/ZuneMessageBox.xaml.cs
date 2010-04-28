using System;
using System.Windows;
using System.Windows.Media.Imaging;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ZuneMessageBox.xaml
    /// </summary>
    public partial class ZuneMessageBox : DraggableWindow
    {
        private readonly Action _okClickedCallback;

        public static void Show(ErrorMessage message, Action okClickedCallback)
        {
            new ZuneMessageBox(message.Message, message.ErrorMode,okClickedCallback).Show();
        }

        public ZuneMessageBox(string errorMessage, ErrorMode mode)
        {
            InitializeComponent();

            switch (mode)
            {
                case ErrorMode.Error:
                    tbMessageTitle.Text = "ERROR";
                    break;
                case ErrorMode.Warning:
                    tbMessageTitle.Text = "WARNING";
                    break;
                case ErrorMode.Info:
                    tbMessageTitle.Text = "INFO";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.imgErrorIcon.Source = new BitmapImage(new Uri(GetIconUriForErrorMode(mode), UriKind.RelativeOrAbsolute));
            this.tbErrorMessage.Text = errorMessage;
            this.tbErrorMessage.ToolTip = errorMessage;
        }

        public ZuneMessageBox(string errorMessage,ErrorMode mode, Action okClickedCallback) :this(errorMessage,mode)
        {
            _okClickedCallback = okClickedCallback;
        }

        private string GetIconUriForErrorMode(ErrorMode mode)
        {
            switch (mode)
            {
                case ErrorMode.Error:
                    return "pack://application:,,,/Assets/error.png";
                case ErrorMode.Warning:
                    return "pack://application:,,,/Assets/alert.png";
                case ErrorMode.Info:
                    return "pack://application:,,,/Assets/info_big.png";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            if (_okClickedCallback != null)
                _okClickedCallback.Invoke();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

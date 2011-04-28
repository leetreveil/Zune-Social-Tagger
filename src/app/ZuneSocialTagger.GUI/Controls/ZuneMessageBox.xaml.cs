using System;
using System.Windows;
using System.Windows.Media.Imaging;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ZuneMessageBox.xaml
    /// </summary>
    public partial class ZuneMessageBox : DraggableWindow
    {
        private readonly Action _okClickedCallback;
        public MessageBoxResult result;

        public ZuneMessageBox()
        {
            this.Owner = System.Windows.Application.Current.MainWindow;
        }

        public static MessageBoxResult Show(ErrorMessage message, MessageBoxButton buttons)
        {
            var msgBox = new ZuneMessageBox(message.Message, message.ErrorMode, buttons);
            msgBox.ShowDialog();
            return msgBox.result;
        }

        public static void Show(ErrorMessage message, Action okClickedCallback)
        {
            new ZuneMessageBox(message.Message, message.ErrorMode,okClickedCallback).Show();
        }

        public static void Show(ErrorMessage message, MessageBoxButton buttons,  Action okClickedCallback)
        {
            new ZuneMessageBox(message.Message, message.ErrorMode, okClickedCallback).Show();
        }

        public ZuneMessageBox(string errorMessage, ErrorMode mode, Action okClickedCallback)
            : this(errorMessage, mode, MessageBoxButton.OKCancel)
        {
            _okClickedCallback = okClickedCallback;
        }

        public ZuneMessageBox(string errorMessage, ErrorMode mode, MessageBoxButton buttons) : this()
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

            if (buttons == MessageBoxButton.YesNo)
            {
                btnOk.Content = "YES";
                btnCancel.Content = "NO";
            }

            this.imgErrorIcon.Source = new BitmapImage(new Uri(GetIconUriForErrorMode(mode), UriKind.RelativeOrAbsolute));
            this.tbErrorMessage.Text = errorMessage;
            this.tbErrorMessage.ToolTip = errorMessage;
        }

        private string GetIconUriForErrorMode(ErrorMode mode)
        {
            switch (mode)
            {
                case ErrorMode.Error:
                    return "pack://application:,,,/Resources/Assets/error.png";
                case ErrorMode.Warning:
                    return "pack://application:,,,/Resources/Assets/alert.png";
                case ErrorMode.Info:
                    return "pack://application:,,,/Resources/Assets/info_big.png";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            this.Close();
        }
    }
}

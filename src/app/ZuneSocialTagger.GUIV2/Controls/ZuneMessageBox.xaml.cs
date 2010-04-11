using System;
using System.Windows;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Controls
{
    public enum ZuneMessageBoxButton
    {
        OK,
        OKCancel
    }

    /// <summary>
    /// Interaction logic for ZuneMessageBox.xaml
    /// </summary>
    public partial class ZuneMessageBox : DraggableWindow
    {
        private readonly string _errorMessage;
        private readonly ErrorMode _mode;
        private readonly ZuneMessageBoxButton _buttonMode;
        private readonly Action _okClickedCallback;

        public static void Show(ErrorMessage message)
        {
            new ZuneMessageBox(message.Message, message.ErrorMode).Show();
        }
        public static void Show(ErrorMessage message, ZuneMessageBoxButton buttonMode, Action okClickedCallback)
        {
            new ZuneMessageBox(message.Message, message.ErrorMode, buttonMode, okClickedCallback).Show();
        }

        public ZuneMessageBox(string errorMessage, ErrorMode mode)
        {
            InitializeComponent();

            _errorMessage = errorMessage;
            _mode = mode;

            this.DataContext = this;
        }

        public ZuneMessageBox(string errorMessage,ErrorMode mode, ZuneMessageBoxButton buttonMode,Action okClickedCallback) :this(errorMessage,mode)
        {
            _buttonMode = buttonMode;
            _okClickedCallback = okClickedCallback;
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public ZuneMessageBoxButton ButtonMode
        {
            get { return _buttonMode; }
        }

        public string MessageTitle
        {
            get
            {
                switch (_mode)
                {
                    case ErrorMode.Error:
                        return "ERROR";
                    case ErrorMode.Warning:
                        return "WARNING";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string ImageSourceUrl
        {
            get
            {
                switch (_mode)
                {
                    case ErrorMode.Error:
                        return "pack://application:,,,/Assets/error.png";
                    case ErrorMode.Warning:
                        return "pack://application:,,,/Assets/alert.png";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (_buttonMode == ZuneMessageBoxButton.OK)
                this.Close();
            else
            {
                this.Close();
                _okClickedCallback.Invoke();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

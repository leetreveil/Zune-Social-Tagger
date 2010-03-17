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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Controls
{
    /// <summary>
    /// Interaction logic for InlineZuneMessage.xaml
    /// </summary>
    public partial class InlineZuneMessage : UserControl
    {
        public InlineZuneMessage()
        {
            InitializeComponent();
        }

        public ErrorMode Mode
        {
            get { return (ErrorMode) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public string Message
        {
            get { return (string) GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof (string), typeof (InlineZuneMessage), new PropertyMetadata(MessageValueChanged));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof (ErrorMode), typeof (InlineZuneMessage), new PropertyMetadata(ModeValueChanged));

        private static void MessageValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (InlineZuneMessage) d;
            view.tbError.Text = (string) e.NewValue;
        }

        private static void ModeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (InlineZuneMessage) d;

            switch ((ErrorMode) e.NewValue)
            {
                case ErrorMode.Error:
                    view.imgError.Source =
                        new BitmapImage(new Uri("pack://application:,,,/Assets/stop.png", UriKind.RelativeOrAbsolute));
                    break;
                case ErrorMode.Warning:
                    view.imgError.Source =
                        new BitmapImage(new Uri("pack://application:,,,/Assets/warning.png", UriKind.RelativeOrAbsolute));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
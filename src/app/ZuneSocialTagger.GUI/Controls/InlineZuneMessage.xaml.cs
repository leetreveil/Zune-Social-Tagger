using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ZuneSocialTagger.GUI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;

namespace ZuneSocialTagger.GUI.Controls
{
    /// <summary>
    /// Interaction logic for InlineZuneMessage.xaml
    /// </summary>
    public partial class InlineZuneMessage : UserControl
    {
        private readonly Timer _timer;
        private Queue<ErrorMessage> _messages;

        public InlineZuneMessage()
        {
            InitializeComponent();
            this.Visibility = Visibility.Collapsed;

            _timer = new Timer();
            _timer.Elapsed += delegate 
            {
                DispatcherHelper.CheckBeginInvokeOnUI(CheckMessages);
            };

            _messages = new Queue<ErrorMessage>();
        }

        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<ErrorMessage>), typeof(InlineZuneMessage),
                                        new PropertyMetadata(MessagesChanged));                
                                        
        public ObservableCollection<ErrorMessage> Messages
        {
            get { return (ObservableCollection<ErrorMessage>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        private static void MessagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (InlineZuneMessage)d;
            var list = (SafeObservableCollection<ErrorMessage>)e.NewValue;

            //if there are any messages in the list before
            //we have a chance to see them push them to the view
            foreach (var errorMessage in list)
                view.DisplayMessage(errorMessage);

            list.CollectionChanged += (sender, value) => 
                view.QueueMessage((ErrorMessage) value.NewItems[0]);
        }

        private void QueueMessage(ErrorMessage message)
        {
            _messages.Enqueue(message);

            if (Visibility == Visibility.Collapsed)
                DisplayMessage(_messages.Dequeue());
        }

        private void DisplayMessage(ErrorMessage message) {
            this.Visibility = Visibility.Visible;
            SetMessageText(message.Message);

            var imageResourcePath = GetErrorImageFor(message.ErrorMode);
            this.imgError.Source = new BitmapImage(new Uri(imageResourcePath, UriKind.RelativeOrAbsolute));

            this._timer.Interval = 20000;
            this._timer.Start();
        }

        private void SetMessageText(string message)
        {
            //look in the string for a hyperlink first
            var split = message.Split('|');
            tbMessage.Text = split[0];

            if (split.Length > 1)
            {
                var url = split[1];
                var msg = split[2];

                hlAddress.NavigateUri = new Uri(url);
                hlText.Text = msg;
            }
        }

        private void CheckMessages() 
        {
            if (_messages.Count == 0)
                this.Visibility = Visibility.Collapsed;
            else
                DisplayMessage(_messages.Dequeue());
        }

        private static string GetErrorImageFor(ErrorMode errorMode)
        {
            switch (errorMode)
            {
                case ErrorMode.Error:
                    return "../Resources/Assets/no.png";
                case ErrorMode.Warning:
                    return "../Resources/Assets/warning.png";
                case ErrorMode.Info:
                    return "../Resources/Assets/information.png";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            CheckMessages();
        }

        private void hlAddress_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(hlAddress.NavigateUri.ToString());
        }
    }
}
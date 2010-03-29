using System.Windows;
using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Timers;
using System;
using Timer = System.Timers.Timer;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for InlineZuneMessageView.xaml
    /// </summary>
    public partial class InlineZuneMessageView : UserControl
    {
        Timer timer = new Timer();

        public InlineZuneMessageView()
        {
            InitializeComponent();

            var model = (InlineZuneMessageViewModel) this.DataContext;
            model.DoShowMessage +=model_DoShowMessage;

            timer.Elapsed += TimerElapsed;
        }


        void model_DoShowMessage()
        {
            timer.Interval = 10000;
            timer.Start();

            this.WarningBox.Visibility = Visibility.Visible;
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate { this.WarningBox.Visibility = Visibility.Collapsed; }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WarningBox.Visibility = Visibility.Collapsed;
        }
    }

}
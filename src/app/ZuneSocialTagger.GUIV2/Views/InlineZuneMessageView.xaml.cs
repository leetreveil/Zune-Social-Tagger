using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Diagnostics;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Timers;
using System;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for InlineZuneMessageView.xaml
    /// </summary>
    public partial class InlineZuneMessageView : UserControl
    {
        System.Timers.Timer t = new System.Timers.Timer();

        public InlineZuneMessageView()
        {
            InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(InlineZuneMessageView_DataContextChanged);
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
        }

        void InlineZuneMessageView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.GetType() == typeof(InlineZuneMessageViewModel))
            {
                var model = (InlineZuneMessageViewModel)this.DataContext;
                model.DoShowMessage += new System.Action(model_DoShowMessage); 
            }
        }

        void model_DoShowMessage()
        {
            t.Interval = 10000;
            t.Start();

            this.WarningBox.Visibility = Visibility.Visible;
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate { this.WarningBox.Visibility = Visibility.Collapsed; }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WarningBox.Visibility = Visibility.Collapsed;
        }
    }

}
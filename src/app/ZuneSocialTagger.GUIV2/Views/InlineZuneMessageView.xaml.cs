using System;
using System.Windows;
using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for InlineZuneMessageView.xaml
    /// </summary>
    public partial class InlineZuneMessageView : UserControl
    {
        public InlineZuneMessageView()
        {
            InitializeComponent();

            var model = (InlineZuneMessageViewModel) this.DataContext;

            model.ShowMessages += () => { this.RootVisual.Visibility = Visibility.Visible; };
            model.HideMessages += () => Dispatcher.Invoke(new Action(() =>
                                                                         {
                                                                             this.RootVisual.Visibility =
                                                                                 Visibility.Collapsed;
                                                                         }));
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.RootVisual.Visibility = Visibility.Collapsed;
        }
    }

}
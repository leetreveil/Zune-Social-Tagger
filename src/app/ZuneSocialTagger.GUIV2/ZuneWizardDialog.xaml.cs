using System;
using System.Windows;
using System.Windows.Input;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for ZuneWizardDialog.xaml
    /// </summary>
    public partial class ZuneWizardDialog : Window
    {
        private readonly ZuneWizardViewModel _zuneWizardViewModel;

        public ZuneWizardDialog()
        {
            InitializeComponent();

            _zuneWizardViewModel = new ZuneWizardViewModel();
            base.DataContext = _zuneWizardViewModel;   
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

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
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : DraggableWindow
    {
        private readonly UpdateViewModel _updateViewModel;

        public UpdateView(UpdateViewModel updateViewModel)
        {
            InitializeComponent();

            _updateViewModel = updateViewModel;
            this.DataContext = _updateViewModel;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainPage = new ZuneWizardDialog();

            mainPage.Show();
            mainPage.Left = this.Left;
            mainPage.Top = this.Top;

            this.Close();
        }
    }
}

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
using leetreveil.AutoUpdate.Framework;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for ZuneWizardView.xaml
    /// </summary>
    public partial class ZuneWizardView : UserControl
    {
        public ZuneWizardView()
        {
            InitializeComponent();
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //TODO: move the updateview so its part of the wizard as is not standalone
            var parentWindow = Window.GetWindow(this);

            var updView = new UpdateView(new UpdateViewModel(UpdateManager.NewUpdate.Version), parentWindow)
                              {
                                  Top = parentWindow.Top,
                                  Left = parentWindow.Left
                              };

            updView.Show();

            parentWindow.Hide();
        }
    }
}

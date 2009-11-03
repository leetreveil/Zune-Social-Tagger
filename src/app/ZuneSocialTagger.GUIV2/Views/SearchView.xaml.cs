using System;
using System.Collections.Generic;
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
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            this.InitializeComponent();
			
            // Insert code required on object creation below this point.
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var dataContext = (SearchViewModel)this.DataContext;

            dataContext.ViewShown();
        }
    }
}
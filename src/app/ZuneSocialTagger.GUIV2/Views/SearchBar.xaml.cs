using System;
using System.Windows.Controls;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Windows.Input;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for SearchBar.xaml
    /// </summary>
    public partial class SearchBar : UserControl
    {
        public SearchBar()
        {
            this.InitializeComponent();
        }

        private void tbSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //TODO: do this through databinding
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                var context = (SearchBarViewModel) this.DataContext;

                context.Search();
            }
        }
    }
}
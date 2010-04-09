using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZuneSocialTagger.GUIV2.Controls
{
    /// <summary>
    /// Interaction logic for SearchBarView.xaml
    /// </summary>
    public partial class SearchBarControl : UserControl
    {
        public event Action SearchClicked = delegate { };

        public SearchBarControl()
        {
            this.InitializeComponent();
            this.tbSearching.Visibility = Visibility.Hidden;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.tbSearch.Text.Length > 0 && e.Key == Key.Enter)
                SearchClicked.Invoke();

            this.SearchText = this.tbSearch.Text;
        }

        private void Search_Clicked(object sender, RoutedEventArgs e)
        {
            SearchClicked.Invoke();
        }

        public static readonly DependencyProperty IsSearchingProperty =
            DependencyProperty.Register("IsSearching", typeof (bool), typeof (SearchBarControl),
                                        new PropertyMetadata(false, IsSearchingChangedCallback));

        private static void IsSearchingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = (SearchBarControl) d;

            thisControl.tbSearching.Visibility = (bool) e.NewValue
                                                     ? Visibility.Visible
                                                     : Visibility.Hidden;
        }

        public bool IsSearching
        {
            get { return (bool) GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof (string), typeof (SearchBarControl),
                                        new PropertyMetadata("",SearchTextChangedCallback));

        private static void SearchTextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = (SearchBarControl)d;

            thisControl.tbSearch.Text = (string) e.NewValue;
        }

        public string SearchText
        {
            get { return (string) GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
    }
}
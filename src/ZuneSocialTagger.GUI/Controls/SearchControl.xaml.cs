using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ZuneSocialTagger.GUI.Controls
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : TextBox
    {
        public SearchControl()
        {
            InitializeComponent();
        }


        public static DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command", 
                typeof(ICommand), 
                typeof(SearchControl),
                new PropertyMetadata(null));


        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof (object),
                typeof (SearchControl),
                new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty,value);}
        }

        public ICommand Command
        {
            get { return GetValue(CommandProperty) as ICommand; }
            set { SetValue(CommandProperty, value); }
        }

        protected virtual void OnSearchClicked()
        {
            if (Command != null && Command.CanExecute(null))
                Command.Execute(CommandParameter);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.tbSearch.Text.Length > 0 && e.Key == Key.Enter)
                OnSearchClicked();
        }

        private void Search_Clicked(object sender, RoutedEventArgs e)
        {
            OnSearchClicked();
        }
    }
}

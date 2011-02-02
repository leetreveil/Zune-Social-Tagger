using System.Windows;
using System.Windows.Controls;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search
{
    /// <summary>
    /// RadioButton that IsChecked can be bound to correctly
    /// </summary>
    public class RadioButtonExtended : RadioButton
    {
        static bool _mBIsChanging;

        public RadioButtonExtended()
        {
            this.Checked += RadioButtonExtended_Checked;
            this.Unchecked += RadioButtonExtended_Unchecked;
        }

        void RadioButtonExtended_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_mBIsChanging)
                this.IsCheckedReal = false;
        }

        void RadioButtonExtended_Checked(object sender, RoutedEventArgs e)
        {
            if (!_mBIsChanging)
                this.IsCheckedReal = true;
        }

        public bool IsCheckedReal
        {
            get { return (bool)GetValue(IsCheckedRealProperty); }
            set
            {
                SetValue(IsCheckedRealProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsCheckedReal. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedRealProperty =
        DependencyProperty.Register("IsCheckedReal", typeof(bool?), typeof(RadioButtonExtended),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal |
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,IsCheckedRealChanged));

        public static void IsCheckedRealChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _mBIsChanging = true;
            ((RadioButtonExtended)d).IsChecked = (bool)e.NewValue;
            _mBIsChanging = false;
        }
    }
}
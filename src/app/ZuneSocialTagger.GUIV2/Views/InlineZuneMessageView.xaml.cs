using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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

            this.WarningBox.IsVisibleChanged += WarningBox_IsVisibleChanged;
        }

        void WarningBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool) e.NewValue)) return;


            var animation = (Storyboard) FindResource("SizeFadeInOut");
            animation.Completed += delegate
                                       {
                                           this.WarningBox.Visibility = Visibility.Collapsed;
                                       };
            animation.Begin();
        }
    }
}
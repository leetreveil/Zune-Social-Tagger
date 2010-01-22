using System.Windows;

namespace ZuneSocialTagger.GUIV2
{
    public class DraggableWindow : Window
    {
        public DraggableWindow()
        {
            this.MouseLeftButtonDown += PageViewBase_MouseLeftButtonDown;
        }

        void PageViewBase_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
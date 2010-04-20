using System.Windows;

namespace ZuneSocialTagger.GUI
{
    public class DraggableWindow :  Window
    {
        public DraggableWindow()
        {
            this.MouseLeftButtonDown += (sender, args) => this.DragMove();
        }
    }
}
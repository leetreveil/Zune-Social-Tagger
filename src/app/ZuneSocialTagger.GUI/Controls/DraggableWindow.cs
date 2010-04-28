using System.Windows;

namespace ZuneSocialTagger.GUI.Controls
{
    public class DraggableWindow :  Window
    {
        public DraggableWindow()
        {
            this.MouseLeftButtonDown += (sender, args) => this.DragMove();
        }
    }
}
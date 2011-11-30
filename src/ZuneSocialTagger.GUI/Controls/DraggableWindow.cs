using System.Windows;
using System;

namespace ZuneSocialTagger.GUI.Controls
{
    public class DraggableWindow :  Window
    {
        public DraggableWindow()
        {
            try
            {
                this.MouseLeftButtonDown += (sender, args) => this.DragMove();
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}
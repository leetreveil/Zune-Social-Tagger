﻿using System.Windows;
using ZuneSocialTagger.GUI.Controls;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Update
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : DraggableWindow
    {
        public UpdateView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZuneSocialTagger.GUI
{
	/// <summary>
	/// Interaction logic for SelectAudioFilesView.xaml
	/// </summary>
	public partial class SelectAudioFilesView : Window
	{
		public SelectAudioFilesView()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

		private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.DragMove();
		}
	}
}
using System.Windows;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : DraggableWindow
    {
        private readonly UpdateViewModel _updateViewModel;
        private readonly Window _mainWindow;

        public UpdateView(UpdateViewModel updateViewModel, Window mainWindow)
        {
            InitializeComponent();

            _updateViewModel = updateViewModel;
            _mainWindow = mainWindow;
            this.DataContext = _updateViewModel;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Left = this.Left;
            _mainWindow.Top = this.Top;
            _mainWindow.Show();
            this.Close();
        }
    }
}

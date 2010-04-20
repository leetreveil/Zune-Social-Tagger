using System.Windows;
using System.Windows.Input;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Views
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : DraggableWindow
    {
        private readonly ApplicationViewModel _viewModel;

        public ApplicationView(ApplicationViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            this.DataContext = viewModel;
            this.Loaded += ApplicationView_Loaded;
            Application.Current.Exit += Current_Exit;
        }

        void ApplicationView_Loaded(object sender, RoutedEventArgs e)
        {
           _viewModel.ApplicationViewHasLoaded();
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            _viewModel.ApplicationIsShuttingDown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
           Application.Current.Shutdown();
        }
    }
}

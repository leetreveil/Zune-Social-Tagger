using System.Windows;
using System.Windows.Input;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : DraggableWindow
    {
        private ViewModelBase _currentViewModel;

        public ApplicationView(ViewLocator locator)
        {
            locator.SwitchToViewRequested += (arg1, arg2) => _currentViewModel = arg2;
            InitializeComponent();
        }

        void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void NavigationCommands_BrowseForward(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentViewModel is Search.SearchViewModel)
            {
                var svm = (Search.SearchViewModel)_currentViewModel;
                svm.MoveNextCommand.Execute(null);
            }

            if (_currentViewModel is Success.SuccessViewModel)
            {
                var svm = (Success.SuccessViewModel)_currentViewModel;
                svm.OKCommand.Execute(null);
            }
        }
        
        private void NavigationCommands_BrowseBack(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentViewModel is Search.SearchViewModel)
            {
                var svm = (Search.SearchViewModel)_currentViewModel;
                svm.MoveBackCommand.Execute(null);
            }

            if (_currentViewModel is Details.DetailsViewModel)
            {
                var dvm = (Details.DetailsViewModel)_currentViewModel;
                dvm.MoveBackCommand.Execute(null);
            }
        }
    }
}

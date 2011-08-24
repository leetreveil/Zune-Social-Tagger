using System.Windows;
using System.Windows.Input;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : DraggableWindow
    {
        private readonly ViewLocator _locator;

        public ApplicationView(ViewLocator locator)
        {
            _locator = locator;
            InitializeComponent();
        }

        void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void NavigationCommands_BrowseForward(object sender, ExecutedRoutedEventArgs e)
        {
            if (_locator.CurrentViewModel is Search.SearchViewModel)
            {
                var svm = (Search.SearchViewModel)_locator.CurrentViewModel;
                svm.MoveNextCommand.Execute(null);
            }

            if (_locator.CurrentViewModel is Success.SuccessViewModel)
            {
                var svm = (Success.SuccessViewModel)_locator.CurrentViewModel;
                svm.OKCommand.Execute(null);
            }
        }
        
        private void NavigationCommands_BrowseBack(object sender, ExecutedRoutedEventArgs e)
        {
            if (_locator.CurrentViewModel is Search.SearchViewModel)
            {
                var svm = (Search.SearchViewModel)_locator.CurrentViewModel;
                svm.MoveBackCommand.Execute(null);
            }

            if (_locator.CurrentViewModel is Details.DetailsViewModel)
            {
                var dvm = (Details.DetailsViewModel)_locator.CurrentViewModel;
                dvm.MoveBackCommand.Execute(null);
            }
        }
    }
}

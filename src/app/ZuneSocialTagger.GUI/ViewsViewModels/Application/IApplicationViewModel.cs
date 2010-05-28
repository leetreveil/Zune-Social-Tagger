using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{
    public interface IApplicationViewModel
    {
        RelayCommand UpdateCommand { get; }
        RelayCommand ShowAboutSettingsCommand { get; }
        RelayCommand CloseAppCommand { get; }
        string ErrorMessageText { get; set; }
        ErrorMode ErrorMessageMode { get; set; }
        bool ShouldShowErrorMessage { get; set; }
        bool UpdateAvailable { get; set; }
        ViewModelBase CurrentPage { get; }

        void ViewHasLoaded();
        void DisplayMessage(ErrorMessage message);
        void AlbumBeenLinked();
    }
}
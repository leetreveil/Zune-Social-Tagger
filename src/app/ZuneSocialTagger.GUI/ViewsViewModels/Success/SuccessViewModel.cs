using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success
{
    public class SuccessViewModel : ViewModelBase
    {
        public SuccessViewModel()
        {
            this.AlbumDetailsFromFile = ApplicationViewModel.AlbumDetailsFromFile;
            this.AlbumDetailsFromWebsite = ApplicationViewModel.AlbumDetailsFromWeb;

            this.OKCommand =new RelayCommand(() => {
                Messenger.Default.Send<string, ApplicationViewModel>("SWITCHTOFIRSTVIEW");
                Messenger.Default.Send<string, ApplicationViewModel>("ALBUMLINKED");
            });
        }

        public RelayCommand OKCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; private set; }
    }
}
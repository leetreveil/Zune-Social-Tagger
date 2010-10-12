using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success
{
    public class SuccessViewModel : ViewModelBase
    {
        public SuccessViewModel(IViewModelLocator locator, SharedModel sharedModel)
        {
            this.AlbumDetailsFromFile = sharedModel.AlbumDetailsFromFile;
            this.AlbumDetailsFromWebsite = sharedModel.AlbumDetailsFromWeb;
            this.OKCommand = new RelayCommand(locator.SwitchToFirstViewModel);
        }

        public RelayCommand OKCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; private set; }
    }
}
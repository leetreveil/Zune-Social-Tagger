using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success
{
    public class SuccessViewModel : ViewModelBase
    {
        public SuccessViewModel(IViewModelLocator locator)
        {
            //this.AlbumDetailsFromFile = avm.AlbumDetailsFromFile;
            //this.AlbumDetailsFromWebsite = avm.AlbumDetailsFromWeb;

            this.OKCommand = new RelayCommand(locator.SwitchToFirstViewModel);
        }

        public RelayCommand OKCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; private set; }
    }
}
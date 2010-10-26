using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success
{
    public class SuccessViewModel : ViewModelBase
    {
        public SuccessViewModel(IViewModelLocator locator, SharedModel sharedModel)
        {
            this.AlbumDetailsFromFile = sharedModel.AlbumDetailsFromFile;
            this.AlbumDetailsFromWebsite = sharedModel.AlbumDetailsFromWeb;
            this.OKCommand = new RelayCommand(delegate
            {
                var webAlbumListViewModel = locator.SwitchToViewModel<WebAlbumListViewModel>();
                webAlbumListViewModel.SelectedAlbum.RefreshAlbum();
            });
        }

        public RelayCommand OKCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; private set; }
    }
}
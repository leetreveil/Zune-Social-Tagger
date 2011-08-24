using System.Windows.Input;
using Ninject;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success
{
    public class SuccessViewModel : ViewModelBase
    {
        private readonly ViewLocator _locator;
        private readonly SharedModel _sharedModel;
        private readonly IKernel _kernel;

        public SuccessViewModel(ViewLocator locator, SharedModel sharedModel, IKernel kernel)
        {
            _locator = locator;
            _sharedModel = sharedModel;
            _kernel = kernel;
        }

        private ICommand _okCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                    _okCommand = new RelayCommand(RefreshAlbum);

                return _okCommand;
            }
        }

        private ExpandedAlbumDetailsViewModel _albumDetailsFromFile;
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile
        {
            get
            {
                if (_albumDetailsFromFile == null)
                    _albumDetailsFromFile = _sharedModel.AlbumDetailsFromFile;

                return _albumDetailsFromFile;
            }
        }

        private ExpandedAlbumDetailsViewModel _albumDetailsFromWebsite;
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite
        {
            get
            {
                if (_albumDetailsFromWebsite == null)
                    _albumDetailsFromWebsite = _sharedModel.AlbumDetailsFromWeb;

                return _albumDetailsFromWebsite;
            }
        }

        private void RefreshAlbum()
        {
            var view = _locator.SwitchToFirstView();

            if (view is WebAlbumListView)
            {
                var webAlbumListViewModel = _kernel.Get<WebAlbumListViewModel>();

                if (webAlbumListViewModel.SelectedAlbum != null)
                    webAlbumListViewModel.SelectedAlbum.RefreshAlbum();
            }
        }
    }
}
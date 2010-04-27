using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SuccessViewModel : ViewModelBaseExtended
    {
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromWebsite;
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromFile;

        public SuccessViewModel(ZuneWizardModel model)
        {
            _albumDetailsFromWebsite = model.SelectedAlbum.WebAlbumMetaData;
            _albumDetailsFromFile = model.SelectedAlbum.ZuneAlbumMetaData;

            this.OKCommand =new RelayCommand(() => Messenger.Default.Send("SWITCHTOFIRSTVIEW"));
        }

        public RelayCommand OKCommand { get; private set; }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite
        {
            get { return _albumDetailsFromWebsite; }
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile
        {
            get { return _albumDetailsFromFile; }
        }
    }
}
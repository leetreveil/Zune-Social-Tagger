using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SuccessViewModel : ViewModelBase
    {
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromWebsite;
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromFile;

        public SuccessViewModel(IZuneWizardModel model)
        {
            _albumDetailsFromWebsite = model.SelectedAlbum.WebAlbumMetaData;
            _albumDetailsFromFile = model.SelectedAlbum.ZuneAlbumMetaData;

            this.OKCommand =new RelayCommand(() => Messenger.Default.Send(typeof(DetailsViewModel)));
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
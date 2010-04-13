using GalaSoft.MvvmLight;
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
        }

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
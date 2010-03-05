using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SuccessViewModel
    {
        private readonly IZuneWizardModel _model;

        public SuccessViewModel(IZuneWizardModel model)
        {
            _model = model;
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite
        {
            get { return _model.AlbumDetailsFromWebsite; }
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile
        {
            get { return _model.AlbumDetailsFromFile; }
        }
    }
}
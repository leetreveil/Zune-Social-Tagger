using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SuccessViewModel
    {
        private readonly ZuneWizardModel _model;

        public SuccessViewModel(ZuneWizardModel model)
        {
            _model = model;
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite
        {
            get { return _model.AlbumDetailsFromWebsite; }
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _model.AlbumDetailsFromFile; }
        }
    }
}
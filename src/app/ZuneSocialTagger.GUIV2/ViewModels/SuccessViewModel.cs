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
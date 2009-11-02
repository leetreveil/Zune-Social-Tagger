using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WebsiteAlbumMetaDataViewModel
    {
        private ZuneNetAlbumMetaData _metaData;

        public WebsiteAlbumMetaDataViewModel()
        {
            ZuneWizardModel.GetInstance().AlbumMetaDataChanged += ModelAlbumMetaDataChanged;
        }

        void ModelAlbumMetaDataChanged(ZuneNetAlbumMetaData obj)
        {
            this._metaData = obj;
        }

        public string Title
        {
            get { return _metaData.Title; }
        }

        public string Artist
        {
            get { return _metaData.Artist; }
        }

        public string SongCount
        {
            get { return _metaData.SongCount + " songs" ; }
        }

        public string Year
        {
            get { return _metaData.Year; }
        }

        public string ArtworkUrl
        {
            get 
            {
                return _metaData.ArtworkUrl ?? "../Assets/blankartwork.png";
            }
        }
    }
}
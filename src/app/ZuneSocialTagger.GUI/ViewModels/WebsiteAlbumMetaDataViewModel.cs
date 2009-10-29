using ZuneSocialTagger.GUI.Models;



namespace ZuneSocialTagger.GUI.ViewModels
{
    public class WebsiteAlbumMetaDataViewModel : ViewModelBase
    {
        private readonly ZuneNetAlbumMetaData _metaData;

        public WebsiteAlbumMetaDataViewModel(ZuneNetAlbumMetaData metaData)
        {
            _metaData = metaData;

            //Defaults
            if (string.IsNullOrEmpty(metaData.ArtworkUrl))
                _metaData.ArtworkUrl = "Assets/blankartwork.png";
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
            get { return _metaData.SongCount; }
        }

        public string Year
        {
            get { return _metaData.Year; }
        }

        public string ArtworkUrl
        {
            get { return _metaData.ArtworkUrl; }
        }
    }
}
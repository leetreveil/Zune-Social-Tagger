namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WebsiteAlbumMetaDataViewModel
    {
        public string Year { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }

        private string _songCount;
        public string SongCount
        {
            get { return _songCount + " songs"; }
            set { _songCount = value; }
        }

        private string _artworkUrl;
        public string ArtworkUrl
        {
            get { return _artworkUrl ?? "../Assets/blankartwork.png"; }
            set { _artworkUrl = value; }
        }
    }
}
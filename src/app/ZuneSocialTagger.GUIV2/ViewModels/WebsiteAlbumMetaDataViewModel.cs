namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WebsiteAlbumMetaDataViewModel
    {
        private string _songCount;
        private string _artworkUrl;

        public string Year { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }


        public string SongCount
        {
            get { return _songCount + " songs"; }
            set { _songCount = value; }
        }

        public string ArtworkUrl
        {
            get { return _artworkUrl ?? @"../Assets/blankartwork.png"; }
            set { _artworkUrl = value; }
        }
    }
}
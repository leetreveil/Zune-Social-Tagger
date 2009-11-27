using System;
using System.Windows.Media.Imaging;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WebsiteAlbumMetaDataViewModel
    {
        private string _songCount;
        private BitmapImage _artworkUrl;

        public string Year { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }


        public string SongCount
        {
            get { return _songCount + " songs"; }
            set { _songCount = value; }
        }

        public BitmapImage ArtworkUrl
        {
            get { return _artworkUrl ?? new BitmapImage(new Uri(@"../Assets/blankartwork.png",UriKind.Relative)); }
            set { _artworkUrl = value; }
        }
    }
}
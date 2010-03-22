using ZuneSocialTagger.GUIV2.Models;
using Album = ZuneSocialTagger.Core.ZuneDatabase.Album;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class AlbumDetailsViewModel : NotifyPropertyChangedImpl
    {
        private Album _zuneAlbumMetaData;
        private Album _webAlbumMetaData;
        private LinkStatus _linkStatus;

        public AlbumDetailsViewModel(AlbumDetails albumDetails)
        {
            this.ZuneAlbumMetaData = albumDetails.ZuneAlbumMetaData;
            this.WebAlbumMetaData = albumDetails.WebAlbumMetaData;
            this.LinkStatus = albumDetails.LinkStatus;
        }

        private AlbumDetailsViewModel()
        {
            //used for serialization purposes
        }

        public Album ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                _zuneAlbumMetaData = value;
                NotifyOfPropertyChange(() => this.ZuneAlbumMetaData);
            }
        }

        public Album WebAlbumMetaData
        {
            get { return _webAlbumMetaData; }
            set
            {
                _webAlbumMetaData = value;
                NotifyOfPropertyChange(() => this.WebAlbumMetaData);
            }
        }

        public LinkStatus LinkStatus
        {
            get { return _linkStatus; }
            set
            {
                _linkStatus = value;
                NotifyOfPropertyChange(() => this.LinkStatus);
            }
        }

        public bool HasDownloaded { get; set; }
    }
}
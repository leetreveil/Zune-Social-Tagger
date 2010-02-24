using Caliburn.Core;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class Album : PropertyChangedBase
    {
        private AlbumMetaData _webAlbumMetaData;
        private AlbumDetails _zuneAlbumMetaData;
        private LinkStatus _linkStatus;

        public LinkStatus LinkStatus
        {
            get { return _linkStatus; }
            set
            {
                _linkStatus = value;
                NotifyOfPropertyChange(() => this.LinkStatus);
            }
        }

        public AlbumDetails ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                    _zuneAlbumMetaData = value;
                    NotifyOfPropertyChange(() => this.ZuneAlbumMetaData);
            }
        }
        public AlbumMetaData WebAlbumMetaData
        {
            get { return _webAlbumMetaData; }
            set
            {
                _webAlbumMetaData = value;
                NotifyOfPropertyChange(() => this.WebAlbumMetaData);
            }
        }
    }
}

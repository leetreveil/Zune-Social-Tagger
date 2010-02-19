using Caliburn.Core;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class Album : PropertyChangedBase
    {
        private AlbumMetaData _webAlbumMetaData;
        private DbAlbumDetails _zuneAlbumMetaData;
        private LinkStatus _isLinked;

        public LinkStatus IsLinked
        {
            get { return _isLinked; }
            set
            {
                _isLinked = value;
                NotifyOfPropertyChange(() => this.IsLinked);
            }
        }

        public DbAlbumDetails ZuneAlbumMetaData
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

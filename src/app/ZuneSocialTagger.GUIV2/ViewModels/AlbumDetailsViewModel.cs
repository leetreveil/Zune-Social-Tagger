using ZuneSocialTagger.GUIV2.Models;
using Album = ZuneSocialTagger.Core.ZuneDatabase.Album;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class AlbumDetailsViewModel : NotifyPropertyChangedImpl
    {
        private Album _zuneAlbumMetaData;
        private Album _webAlbumMetaData;
        private LinkStatus _linkStatus;
        private bool _isFiltered;

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

        public bool IsFiltered
        {
            get { return _isFiltered; }
            set
            {
                _isFiltered = value;
                NotifyOfPropertyChange(() => this.IsFiltered);
            }
        }

        /// <summary>
        /// Set this boolean value when you want the view to refresh the details from the zune database
        /// </summary>
        public bool NeedsRefreshing { get; set; }
    }
}
using System.Collections.Generic;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;

namespace ZuneSocialTagger.GUI.ViewsViewModels.DetailsReadyOnly
{
    public class DetailsReadOnlyViewModel : ViewModelBase
    {
        private readonly IViewModelLocator _locator;
        private string _artist;
        private string _albumTitle;
        private string _releaseYear;
        private string _trackCount;
        private string _genre;

        public DetailsReadOnlyViewModel(IViewModelLocator locator)
        {
            _locator = locator;
            this.Tracks = new List<TrackWithTrackNum>();
            this.LinkCommand = new RelayCommand(Link);
            this.MoveBackCommand = new RelayCommand(MoveBack);
        }

        public RelayCommand MoveBackCommand { get; set; }
        public RelayCommand LinkCommand { get; set; }
        public string ImageUrl { get; set; }
        public List<TrackWithTrackNum> Tracks { get; set; }

        public string TrackCount
        {
            get { return _trackCount; }
            set
            {
                _trackCount = value + " songs";
            }
        }

        public string Genre
        {
            get { return string.IsNullOrEmpty(_genre) ? "Unknown Genre" : _genre; }
            set { _genre = value; }
        }

        public string ReleaseYear
        {
            get { return string.IsNullOrEmpty(_releaseYear) || _releaseYear == "-1" ? "Unknown Year" : _releaseYear; }
            set { _releaseYear = value; }
        }

        public string AlbumTitle
        {
            get { return string.IsNullOrEmpty(_albumTitle) ? "Unknown Title" : _albumTitle; }
            set { _albumTitle = value; }
        }

        public string Artist
        {
            get { return string.IsNullOrEmpty(_artist) ? "Unknown Artist" : _artist; }
            set { _artist = value; }
        }

        private void Link()
        {
            
        }
        
        private void MoveBack()
        {
            _locator.SwitchToViewModel<SearchViewModel>();
        }
    }
}
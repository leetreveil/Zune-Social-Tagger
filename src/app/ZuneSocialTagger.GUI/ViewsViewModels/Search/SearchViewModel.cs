using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Details;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using System.Collections.Generic;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly IViewModelLocator _locator;
        private string _searchText;
        private bool _isSearching;
        private bool _canMoveNext;
        private SearchResultsViewModel _searchResultsViewModel;
        private bool _canShowResults;

        internal IEnumerable<Song> TracksFromFile;

        public SearchViewModel(IViewModelLocator locator)
        {
            _locator = locator;
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.MoveNextCommand = new RelayCommand(MoveNext);
            this.SearchCommand = new RelayCommand(Search);
        }

        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand MoveNextCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetails { get; set; }

        public SearchResultsViewModel SearchResultsViewModel
        {
            get { return _searchResultsViewModel; }
            set
            {
                _searchResultsViewModel = value;
                RaisePropertyChanged(() => this.SearchResultsViewModel);
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => this.SearchText);
            }
        }

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                RaisePropertyChanged(() => this.IsSearching);
            }
        }

        public bool CanMoveNext
        {
            get { return _canMoveNext; }
            set
            {
                _canMoveNext = value;
                RaisePropertyChanged(() => this.CanMoveNext);
            }
        }

        public bool CanShowResults
        {
            get { return _canShowResults; }
            set
            {
                _canShowResults = value;
                RaisePropertyChanged(() => this.CanShowResults);
            }
        }

        public void SearchClicked()
        {
            this.IsSearching = true;
        }

        public void Search()
        {
            this.CanShowResults = false;
            this.CanMoveNext = false;
            this.IsSearching = true;
            this.SearchResultsViewModel = null;

            AlbumSearch.SearchForAlbumAsync(this.SearchText, albums =>
            {
                this.SearchResultsViewModel = new SearchResultsViewModel();

                DispatcherHelper.CheckBeginInvokeOnUI(() => this.SearchResultsViewModel.LoadAlbums(albums));

                this.CanMoveNext = albums.Count() > 0;
                this.CanShowResults = true;

                SearchForArtists();
            });
        }

        private void SearchForArtists()
        {
            ArtistSearch.SearchForAsync(this.SearchText, artists => {
                this.SearchResultsViewModel.LoadArtists(artists);
                this.IsSearching = false;
            });
        }

        public void MoveBack()
        {
            _locator.SwitchToFirstViewModel();
        }

        public void MoveNext()
        {
            WebAlbum downloadedAlbum = _searchResultsViewModel._downloadedAlbum;
            IEnumerable<WebTrack> downloadedTracks = downloadedAlbum.Tracks;
           // var detailsViewModel = _locator.SwitchToViewModel<DetailsViewModel>();

            //do all tracks match, if they do then switch to completed, otherwise switch to selector
            if (DoAllTracksHaveMatchingTrackTitles(downloadedTracks))
            {
                var detailsRoVm = _locator.SwitchToViewModel<DetailsReadyOnly.DetailsReadOnlyViewModel>();

                detailsRoVm.Genre = downloadedAlbum.Genre;
                detailsRoVm.ImageUrl = downloadedAlbum.ArtworkUrl;
                detailsRoVm.ReleaseYear = downloadedAlbum.ReleaseYear;
                detailsRoVm.AlbumTitle = downloadedAlbum.Title;
                detailsRoVm.Artist = downloadedAlbum.Artist;
                detailsRoVm.TrackCount = downloadedAlbum.Tracks.Count().ToString();

                foreach (var downloadedTrack in downloadedTracks)
                {
                    detailsRoVm.Tracks.Add(new TrackWithTrackNum
                    {
                        TrackTitle = downloadedTrack.Title,
                        TrackNumber = downloadedTrack.TrackNumber
                    });
                }
            }
            else
            {
                var detailsVm = _locator.SwitchToViewModel<DetailsViewModel>();
                detailsVm.AlbumDetailsFromFile = this.AlbumDetails;
                detailsVm.AlbumDetailsFromWebsite = _searchResultsViewModel._downloadedAlbum.GetAlbumDetailsFrom();
                detailsVm._tracksFromWeb = _searchResultsViewModel._downloadedAlbum.Tracks;
                detailsVm._tracksFromFile = TracksFromFile;
                detailsVm.PopulateRows();
            }
        }

        public bool DoAllTracksHaveMatchingTrackTitles(IEnumerable<WebTrack> downloadedTracks)
        {
            return
                TracksFromFile.Any(
                    track =>
                    SharedMethods.DoesAlbumTitleMatch(downloadedTracks.Select(x => x.Title), track.MetaData.Title));
        }
    }
}
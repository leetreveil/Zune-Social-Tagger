using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.ViewsViewModels.Details;
using ZuneSocialTagger.GUI.Shared;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly ViewLocator _locator;
        private readonly SharedModel _sharedModel;

        public SearchViewModel(ViewLocator locator, SharedModel sharedModel)
        {
            _locator = locator;
            _sharedModel = sharedModel;
        }

        #region View Bindings

        public ExpandedAlbumDetailsViewModel AlbumDetails 
        { 
            get { return _sharedModel.AlbumDetailsFromFile; } 
        }

        private RelayCommand _moveBackCommand;
        public RelayCommand MoveBackCommand
        {
            get
            {
                if (_moveBackCommand == null)
                    _moveBackCommand = new RelayCommand(() => _locator.SwitchToFirstView());

                return _moveBackCommand;
            }
        }

        private RelayCommand _moveNextCommand;
        public RelayCommand MoveNextCommand
        {
            get
            {
                if (_moveNextCommand == null)
                    _moveNextCommand = new RelayCommand(MoveNext);

                return _moveNextCommand;
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new RelayCommand(() => { SearchImpl(this.SearchText, this.SearchText); });

                return _searchCommand;
            }
        }

        private SearchResultsViewModel _searchResultsViewModel;
        public SearchResultsViewModel SearchResultsViewModel
        {
            get { return _searchResultsViewModel; }
            set
            {
                _searchResultsViewModel = value;
                RaisePropertyChanged(() => this.SearchResultsViewModel);
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => this.SearchText);
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                RaisePropertyChanged(() => this.IsSearching);
            }
        }

        private bool _canMoveNext;
        public bool CanMoveNext
        {
            get { return _canMoveNext; }
            set
            {
                _canMoveNext = value;
                RaisePropertyChanged(() => this.CanMoveNext);
            }
        }

        private bool _canShowResults;
        public bool CanShowResults
        {
            get { return _canShowResults; }
            set
            {
                _canShowResults = value;
                RaisePropertyChanged(() => this.CanShowResults);
            }
        }

        #endregion View Bindings

        public void Search(string artist, string album)
        {
            this.SearchText = string.Format("{0} {1}", album, artist);
            //use the artist as part of the album search for greater accuracy
            SearchImpl(this.SearchText, artist);
        }

        private void SearchImpl(string album, string artist)
        {
            this.CanShowResults = false;
            this.CanMoveNext = false;
            this.IsSearching = true;
            this.SearchResultsViewModel = null;
            this.SearchResultsViewModel = new SearchResultsViewModel(this);

            SearchForAlbums(album);
            SearchForArtists(artist);
        }

        private void SearchForAlbums(string album)
        {
            AlbumSearch.SearchForAlbumAsync(album, albums =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    this.SearchResultsViewModel.LoadAlbums(albums);
                    this.CanShowResults = true;
                    this.IsSearching = false;
                });
            });
        }

        private void SearchForArtists(string artist)
        {
            ArtistSearch.SearchForAsync(artist, artists =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    this.SearchResultsViewModel.LoadArtists(artists);
                });
            });
        }

        public void MoveNext()
        {
            _sharedModel.WebAlbum = _searchResultsViewModel.DownloadedAlbum;
            _locator.SwitchToView<DetailsView, DetailsViewModel>().PopulateRows();
        }
    }
}
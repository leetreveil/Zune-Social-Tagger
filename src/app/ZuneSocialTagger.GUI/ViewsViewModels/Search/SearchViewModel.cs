using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Details;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly IViewLocator _locator;
        private readonly SharedModel _sharedModel;
        private string _searchText;
        private bool _isSearching;
        private bool _canMoveNext;
        private SearchResultsViewModel _searchResultsViewModel;
        private bool _canShowResults;


        public SearchViewModel(IViewLocator locator, SharedModel sharedModel)
        {
            _locator = locator;
            _sharedModel = sharedModel;
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.MoveNextCommand = new RelayCommand(MoveNext);
            this.SearchCommand = new RelayCommand(SearchButtonClicked);
        }

        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand MoveNextCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetails { get { return _sharedModel.AlbumDetailsFromFile; } }

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

        public void SearchButtonClicked()
        {
            SearchImpl(this.SearchText, this.SearchText);
        }

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
                this.SearchResultsViewModel.LoadAlbums(albums);
                //this.CanMoveNext = albums.Count() > 0;
                this.CanShowResults = true;
                this.IsSearching = false;
            });
        }

        private void SearchForArtists(string artist)
        {
            ArtistSearch.SearchForAsync(artist, artists =>
            {
                this.SearchResultsViewModel.LoadArtists(artists);
            });
        }

        public void MoveBack()
        {
            _locator.SwitchToFirstView();
        }

        public void MoveNext()
        {
            _sharedModel.WebAlbum = _searchResultsViewModel._downloadedAlbum;
            _sharedModel.AlbumDetailsFromWeb = SharedMethods.GetAlbumDetailsFrom(_searchResultsViewModel._downloadedAlbum);

            var detailsViewModel = _locator.SwitchToView<DetailsView,DetailsViewModel>();
            detailsViewModel.PopulateRows();
        }
    }
}
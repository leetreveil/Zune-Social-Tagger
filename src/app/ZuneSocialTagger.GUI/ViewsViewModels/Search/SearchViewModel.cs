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
        private readonly IViewModelLocator _locator;
        private readonly SharedModel _sharedModel;
        private string _searchText;
        private bool _isSearching;
        private bool _canMoveNext;
        private SearchResultsViewModel _searchResultsViewModel;
        private bool _canShowResults;


        public SearchViewModel(IViewModelLocator locator, SharedModel sharedModel)
        {
            _locator = locator;
            _sharedModel = sharedModel;
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.MoveNextCommand = new RelayCommand(MoveNext);
            this.SearchCommand = new RelayCommand(Search);
            this.AlbumDetails = sharedModel.AlbumDetailsFromFile;
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

        public void Search(string artist,string album)
        {
            this.SearchText = string.Format("{0} {1}",album,artist);
            Search();
        }

        private void Search()
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
            _sharedModel.WebAlbum = _searchResultsViewModel._downloadedAlbum;
            _sharedModel.AlbumDetailsFromWeb = SharedMethods.GetAlbumDetailsFrom(_searchResultsViewModel._downloadedAlbum);

            var detailsViewModel = _locator.SwitchToViewModel<DetailsViewModel>();
            detailsViewModel.PopulateRows();
        }
    }
}
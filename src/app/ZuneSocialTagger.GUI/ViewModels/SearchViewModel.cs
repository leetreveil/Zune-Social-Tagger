using System;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SearchViewModel : ViewModelBaseExtended
    {
        private string _searchText;
        private bool _isSearching;
        private bool _canMoveNext;
        private SearchResultsViewModel _searchResultsViewModel;
        private bool _canShowResults;

        public SearchViewModel()
        {
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.MoveNextCommand = new RelayCommand(MoveNext);
            this.SearchCommand = new RelayCommand(Search);

            Messenger.Default.Register<string>(this, GotSearchTextFromOtherViewModels);
        }

        private void GotSearchTextFromOtherViewModels(string searchText)
        {
            this.SearchText = searchText;
            Search();
        }

        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand MoveNextCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetails { get { return ApplicationViewModel.AlbumDetailsFromFile; } }

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

            AlbumSearch.SearchForAsync(this.SearchText, albums => {
                this.SearchResultsViewModel = new SearchResultsViewModel();

                DispatcherHelper.CheckBeginInvokeOnUI(() => this.SearchResultsViewModel.LoadAlbums(albums));

                this.CanMoveNext = albums.Count() > 0;
                this.CanShowResults = true;

                SearchForArtists();
            });
        }

        private void SearchForArtists()
        {
            ArtistSearch.SearchForAsync(this.SearchText,artists => {
                this.SearchResultsViewModel.LoadArtists(artists);
                this.IsSearching = false;
            });
        }

        public void MoveBack()
        {
            Messenger.Default.Send<string,ApplicationViewModel>("SWITCHTOFIRSTVIEW");
        }

        public void MoveNext()
        {
            Messenger.Default.Send<Type,ApplicationViewModel>(typeof(DetailsViewModel));
        }
    }
}
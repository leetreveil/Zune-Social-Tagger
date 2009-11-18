using System;
using System.Windows.Input;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchBarViewModel : NotifyPropertyChangedImpl
    {
        private int _searchPageCount;

        public AsyncObservableCollection<AlbumSearchResult> SearchResults { get; set; }
        public event EventHandler StartedSearching;
        public event EventHandler FirstItemsFound;

        public SearchBarViewModel()
        {
            SearchResults = new AsyncObservableCollection<AlbumSearchResult>();
            AlbumSearch.SearchForAsyncCompleted += (() =>
                                                        {
                                                            this.IsSearching = false;
                                                            _searchPageCount = 0;
                                                        });
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                CanSearch = !String.IsNullOrEmpty(_searchText);
                base.InvokePropertyChanged("SearchText");
            }
        }

        private bool _isSearching;
        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;

                this.CanSearch = !value;

                base.InvokePropertyChanged("IsSearching");
            }
        }

        private bool _canSearch;
        public bool CanSearch
        {
            get { return _canSearch; }
            set
            {
                _canSearch = value;
                base.InvokePropertyChanged("CanSearch");
            }
        }

        private RelayCommand<string> _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new RelayCommand<string>(SearchFor);

                return _searchCommand;
            }
        }

        /// <summary>
        /// Way to search without having to click on the magnifying glass
        /// </summary>
        public void Search()
        {
            if (!String.IsNullOrEmpty(SearchText))
                SearchFor(SearchText);
        }

        private void SearchFor(string searchString)
        {
            InvokeStartedSearching();

            this.SearchResults.Clear();
            this.IsSearching = true;

            try
            {
                AlbumSearch.SearchForAsync(searchString, results =>
                {
                    foreach (var result in results)
                        this.SearchResults.Add(result);

                    _searchPageCount++;

                    if (_searchPageCount == 1)
                        InvokeFirstItemsFound();
                });
            }
            catch (PageDownloaderException ex)
            {
                Console.WriteLine("error downloading the album info");
            }

        }

        private void InvokeStartedSearching()
        {
            EventHandler searching = StartedSearching;
            if (searching != null) searching(this, new EventArgs());
        }

        private void InvokeFirstItemsFound()
        {
            EventHandler handler = FirstItemsFound;
            if (handler != null) handler(this, new EventArgs());
        }
    }
}
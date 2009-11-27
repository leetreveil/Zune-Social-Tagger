using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Commands;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchBarViewModel : ViewModelBase
    {
        private int _searchPageCount;
        private string _searchText;
        private bool _isSearching;
        private bool _canSearch;
        private RelayCommand<string> _searchCommand;

        public ObservableCollection<AlbumSearchResult> SearchResults { get; set; }
        public event EventHandler StartedSearching;
        public event EventHandler FirstItemsFound;

        public SearchBarViewModel()
        {
            SearchResults = new ObservableCollection<AlbumSearchResult>();

            AlbumSearch.SearchForAsyncCompleted += (() =>
                {
                    this.IsSearching = false;
                    _searchPageCount = 0;
                });
        }

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

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                if (value != _isSearching)
                {
                    _isSearching = value;
                    this.CanSearch = !value;
                    base.InvokePropertyChanged("IsSearching");
                }
            }
        }

        public bool CanSearch
        {
            get { return _canSearch; }
            set
            {
                if (value != _canSearch)
                {
                    _canSearch = value;
                    base.InvokePropertyChanged("CanSearch");
                }
            }
        }

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

                    base.UIDispatcher.Invoke(new Action(() =>
                        {
                            foreach (var result in results)
                                this.SearchResults.Add(result);
                        }));


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
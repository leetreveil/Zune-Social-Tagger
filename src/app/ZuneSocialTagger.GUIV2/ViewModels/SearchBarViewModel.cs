using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Commands;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchBarViewModel : ViewModelBase
    {
        private string _searchText;
        private bool _isSearching;
        private bool _canSearch;
        private RelayCommand<string> _searchCommand;

        public ObservableCollection<Album> SearchResults { get; set; }
        public event EventHandler StartedSearching;

        public SearchBarViewModel()
        {
            SearchResults = new ObservableCollection<Album>();
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value != _searchText)
                {
                    _searchText = value;
                    CanSearch = !String.IsNullOrEmpty(_searchText);
                    base.InvokePropertyChanged("SearchText");
                }
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
                    _searchCommand = new RelayCommand<string>(searchStr => Search());

                return _searchCommand;
            }
        }

        /// <summary>
        /// Way to search without having to click on the magnifying glass
        /// </summary>
        public void Search()
        {
            InvokeStartedSearching();
            SearchFor(SearchText);
        }

        private void SearchFor(string searchString)
        {
            this.SearchResults.Clear();
            this.IsSearching = true;

            ThreadPool.QueueUserWorkItem(_ =>
                 {
                     IEnumerable<Album> results = AlbumSearch.SearchFor(searchString);

                     base.UIDispatcher.Invoke(new Action(() =>
                     {
                         foreach (var result in results)
                             this.SearchResults.Add(result);

                         this.IsSearching = false;
                     }));
                 });
        }

        private void InvokeStartedSearching()
        {
            EventHandler searching = StartedSearching;
            if (searching != null) searching(this, new EventArgs());
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Commands;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchBarViewModel : INotifyPropertyChanged
    {
        private RelayCommand<string> _searchCommand;
        private bool _isSearching;
        private bool _canSearch;

        public List<AlbumSearchResult> SearchResults { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler FinishedSearching;

        private void InvokeFinishedSearching()
        {
            EventHandler searching = FinishedSearching;
            if (searching != null) searching(this, new EventArgs());
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                CanSearch = !String.IsNullOrEmpty(_searchText);
            }
        }

        public SearchBarViewModel()
        {
            SearchResults = new List<AlbumSearchResult>();
        }

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                OnPropertyChanged("IsSearching");
            }
        }

        public bool CanSearch
        {
            get { return _canSearch; }
            set
            {
                _canSearch = value;
                OnPropertyChanged("CanSearch");
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand<string>(SearchFor);
                }

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
            this.IsSearching = true;

            AlbumSearch.SearchForAsync(searchString, results =>
            {
                SearchResults.AddRange(results);

                this.IsSearching = false;
                InvokeFinishedSearching();
            });
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this,new PropertyChangedEventArgs(propertyName));
        }
    }
}
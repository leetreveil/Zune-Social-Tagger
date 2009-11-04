using System;
using System.ComponentModel;
using System.Windows.Input;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchBarViewModel : INotifyPropertyChanged
    {
        private RelayCommand<string> _searchCommand;
        private bool _isSearching;
        private bool _textBoxValid;

        public AsyncObservableCollection<AlbumSearchResult> SearchResults { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler FinishedSearching;

        private void InvokeFinishedSearching()
        {
            this.IsSearching = false;

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
                TextBoxValid = !String.IsNullOrEmpty(_searchText);
                OnPropertyChanged("SearchText");
            }
        }

        public SearchBarViewModel()
        {
            SearchResults = new AsyncObservableCollection<AlbumSearchResult>();
            AlbumSearch.SearchForAsyncCompleted += InvokeFinishedSearching;
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

        public bool TextBoxValid
        {
            get { return _textBoxValid; }
            set
            {
                _textBoxValid = value;
                OnPropertyChanged("TextBoxValid");
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
            this.SearchResults.Clear();
            this.IsSearching = true;

            AlbumSearch.SearchForAsync(searchString, results =>
                                                         {
                                                             foreach (var result in results)
                                                                 this.SearchResults.Add(result);
                                                         });
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
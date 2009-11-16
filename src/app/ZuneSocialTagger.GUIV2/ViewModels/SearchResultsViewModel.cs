using System;
using System.Collections.ObjectModel;
using System.Linq;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsViewModel : ZuneWizardPageViewModelBase
    {
        private readonly ZuneWizardModel _model;
        private bool _isLoading;
        private AlbumSearchResult _lastLoaded;
        private SearchResultsDetailsViewModel _searchResultsDetailsViewModel;

        public SearchResultsViewModel(ZuneWizardModel model)
        {
            _model = model;
            this.SearchResultsDetailsViewModel = new SearchResultsDetailsViewModel();
            this.SearchBarViewModel.StartedSearching += SearchBarViewModel_StartedSearching;
        }

        void SearchBarViewModel_StartedSearching(object sender, EventArgs e)
        {
            if (base.IsCurrentPage)
                this.SearchResultsDetailsViewModel = null;
        }

        public ObservableCollection<AlbumSearchResult> Albums
        {
            get { return this.SearchBarViewModel.SearchResults; }
        }

        public SearchResultsDetailsViewModel SearchResultsDetailsViewModel
        {
            get { return _searchResultsDetailsViewModel; }
            set
            {
                _searchResultsDetailsViewModel = value;
                base.OnPropertyChanged("SearchResultsDetailsViewModel");
            }
        }

        public string AlbumCount
        {
            get { return String.Format("ALBUMS ({0})", Albums.Count); }
        }

        public SearchBarViewModel SearchBarViewModel
        {
            get { return _model.SearchBarViewModel; }
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _model.AlbumDetailsFromFile; }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public void LoadAlbum(AlbumSearchResult albumArtistAndTitleWithUrl)
        {
            if (_lastLoaded != albumArtistAndTitleWithUrl)
            {
                this.IsLoading = true;
                PageDownloader.DownloadAsync(albumArtistAndTitleWithUrl.Url, ScrapedPage);
            }

            _lastLoaded = albumArtistAndTitleWithUrl;
        }

        private void ScrapedPage(string pageData)
        {
            var scraper = new AlbumWebpageScraper(pageData);

            _model.AlbumDetailsFromWebsite = new WebsiteAlbumMetaDataViewModel
                 {
                     Title = scraper.ScrapeAlbumTitle(),
                     Artist = scraper.ScrapeAlbumArtist(),
                     ArtworkUrl = scraper.ScrapeAlbumArtworkUrl(),
                     Year = scraper.ScrapeAlbumReleaseYear().ToString(),
                     SongCount = scraper.GetSongTitleAndIDs().Count().ToString()
                 };


            AddSelectedSongs(scraper);
            AddRowInfo(scraper);

            this.IsLoading = false;
        }

        private void AddSelectedSongs(AlbumWebpageScraper scraper)
        {
            var searchResults = new SearchResultsDetailsViewModel();

            searchResults.SelectedAlbumTitle = scraper.ScrapeAlbumTitle();
       
            int counter = 0;
            foreach (var song in scraper.GetSongTitleAndIDs())
            {
                counter++;
                searchResults.SelectedAlbumSongs.Add(new SongWithNumberAndGuid
                                           {Number = counter.ToString(), 
                                            Title = song.Title, 
                                            Guid = song.Guid});
            }

            this.SearchResultsDetailsViewModel = searchResults;
        }

        private void AddRowInfo(AlbumWebpageScraper scraper)
        {
            foreach (var row in _model.Rows)
            {
                row.SongsFromWebsite = this.SearchResultsDetailsViewModel.SelectedAlbumSongs;

                var albumArtistGuid = new MediaIdGuid
                                          {
                                              Guid = scraper.ScrapeAlbumArtistID(),
                                              MediaId = MediaIds.ZuneAlbumArtistMediaID
                                          };

                var albumMediaGuid = new MediaIdGuid
                                         {Guid = scraper.ScrapeAlbumMediaID(), MediaId = MediaIds.ZuneAlbumMediaID};


                row.TagContainer.Add(albumArtistGuid);
                row.TagContainer.Add(albumMediaGuid);
            }
        }


        internal override bool IsNextEnabled()
        {
            //TODO: fix bug where the view is not refreshing fast enough to the changes of these properties
            return this.SearchBarViewModel.CanSearch && !this.IsLoading;
        }
    }
}
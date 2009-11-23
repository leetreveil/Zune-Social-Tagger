using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsViewModel : ZuneWizardPageViewModelBase
    {
        private readonly ZuneWizardModel _model;
        private bool _isLoading;
        private string _lastLoaded;
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
                if (value != _searchResultsDetailsViewModel)
                {
                    _searchResultsDetailsViewModel = value;
                    base.InvokePropertyChanged("SearchResultsDetailsViewModel");
                }
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
                if (value != _isLoading)
                {
                    _isLoading = value;
                    base.InvokePropertyChanged("IsLoading");
                }
            }
        }

        public void LoadAlbum(string url)
        {
            if (_lastLoaded != url)
            {
                this.IsLoading = true;
                PageDownloader.DownloadAsync(url, ScrapedPage);
            }

            _lastLoaded = url;
        }

        private void ScrapedPage(string pageData)
        {
            var scraper = new AlbumWebpageScraper(pageData);

            AlbumWebpageScrapeResult scrapeResult = scraper.Scrape();

            if (scrapeResult.IsValid())
            {
                _model.AlbumDetailsFromWebsite = new WebsiteAlbumMetaDataViewModel
                                                     {
                                                         Title = scrapeResult.AlbumTitle,
                                                         Artist = scrapeResult.AlbumArtist,
                                                         ArtworkUrl = scrapeResult.AlbumArtworkUrl,
                                                         Year = scrapeResult.AlbumReleaseYear.ToString(),
                                                         SongCount =
                                                             scrapeResult.SongTitlesAndMediaID.Count().ToString()
                                                     };

                AddSelectedSongs(scrapeResult.SongTitlesAndMediaID, scrapeResult.AlbumTitle);

                AddRowInfo(scrapeResult);
            }
            else
                this.SearchResultsDetailsViewModel = new SearchResultsDetailsViewModel{SelectedAlbumTitle = "Sorry could not get album details"};

            this.IsLoading = false;
        }

        private void AddSelectedSongs(IEnumerable<SongGuid> songGuids, string albumTitle )
        {
            var searchResults = new SearchResultsDetailsViewModel();

            searchResults.SelectedAlbumTitle = albumTitle;
       
            int counter = 0;
            foreach (var song in songGuids)
            {
                counter++;
                searchResults.SelectedAlbumSongs.Add(new SongWithNumberAndGuid
                                           {Number = counter.ToString(), 
                                            Title = song.Title, 
                                            Guid = song.Guid});
            }

            this.SearchResultsDetailsViewModel = searchResults;
        }

        private void AddRowInfo(AlbumWebpageScrapeResult scrapeResult)
        {
            foreach (var row in _model.Rows)
            {
                row.SongsFromWebsite = this.SearchResultsDetailsViewModel.SelectedAlbumSongs;

                var albumArtistGuid = new MediaIdGuid
                                          {
                                              Guid = scrapeResult.AlbumArtistID,
                                              MediaId = MediaIds.ZuneAlbumArtistMediaID
                                          };

                var albumMediaGuid = new MediaIdGuid
                                         {Guid = scrapeResult.AlbumMediaID, MediaId = MediaIds.ZuneAlbumMediaID};


                row.TagContainer.Add(albumArtistGuid);
                row.TagContainer.Add(albumMediaGuid);
            }
        }


        internal override bool IsNextEnabled()
        {
            return true;
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Models;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsViewModel : ZuneWizardPageViewModelBase
    {
        private readonly ZuneWizardModel _model;
        private bool _isLoading;
        private AlbumSearchResult _lastLoaded;

        public SearchResultsViewModel(ZuneWizardModel model)
        {
            _model = model;
            this.SearchResultsDetailsViewModel = new SearchResultsDetailsViewModel();
        }

        public ObservableCollection<AlbumSearchResult> Albums
        {
            get { return this.SearchBarViewModel.SearchResults; }
        }

        private SearchResultsDetailsViewModel _searchResultsDetailsViewModel;
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

                row.AlbumArtistGuid = new MediaIdGuid
                                          {
                                              Guid = scraper.ScrapeAlbumArtistID(),
                                              MediaId = MediaIds.ZuneAlbumArtistMediaID
                                          };

                row.AlbumMediaGuid = new MediaIdGuid
                                         {Guid = scraper.ScrapeAlbumMediaID(), MediaId = MediaIds.ZuneAlbumMediaID};
            }
        }

        internal override bool IsValid()
        {
            return true;
        }

        internal override bool CanMoveNext()
        {
            return true;
        }
    }
}
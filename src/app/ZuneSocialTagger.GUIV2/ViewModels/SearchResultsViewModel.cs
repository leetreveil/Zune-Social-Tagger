using System;
using System.Collections.ObjectModel;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Models;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsViewModel : ZuneWizardPageViewModelBase
    {
        public ObservableCollection<AlbumSearchResult> Albums 
        { 
            get{return this.SearchBarViewModel.SearchResults;}
        }

        public AsyncObservableCollection<SongWithNumberAndGuid> SelectedAlbumSongs { get; set; }

        public string AlbumCount
        {
            get { return String.Format("ALBUMS ({0})", Albums.Count); }
        }

        private string _selectedAlbumTitle;
        public string SelectedAlbumTitle
        {
            get { return _selectedAlbumTitle; }
            set
            {
                _selectedAlbumTitle = value;
                base.OnPropertyChanged("SelectedAlbumTitle");
            }
        }

        private SearchBarViewModel _searchBarViewModel;
        public SearchBarViewModel SearchBarViewModel
        {
            get { return _searchBarViewModel; }
            set
            {
                _searchBarViewModel = value;
                OnPropertyChanged("SearchBarViewModel");
            }
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        public SearchResultsViewModel()
        {
            this.AlbumDetailsFromFile = ZuneWizardModel.GetInstance().AlbumDetailsFromFile;
            this.SelectedAlbumSongs = ZuneWizardModel.GetInstance().SelectedAlbumSongs;
            this.SearchBarViewModel = ZuneWizardModel.GetInstance().SearchBarViewModel;
        }

        public void LoadAlbum(AlbumSearchResult albumArtistAndTitleWithUrl)
        {
            this.IsLoading = true;
            PageDownloader.DownloadAsync(albumArtistAndTitleWithUrl.Url, ScrapedPage);
        }

        private void ScrapedPage(string pageData)
        {
            var albumDetails = ZuneWizardModel.GetInstance().AlbumDetailsFromWebsite;
            ObservableCollection<DetailRow> detailRows = ZuneWizardModel.GetInstance().Rows;

            var scraper = new AlbumWebpageScraper(pageData);

            this.SelectedAlbumTitle = scraper.ScrapeAlbumTitle();
            albumDetails.Title = scraper.ScrapeAlbumTitle();
            albumDetails.Artist = scraper.ScrapeAlbumArtist();
            albumDetails.ArtworkUrl = scraper.ScrapeAlbumArtworkUrl();
            albumDetails.Year = scraper.ScrapeAlbumReleaseYear().ToString();

            SelectedAlbumSongs.Clear();

            //TODO: fix this hack that allows it to clear properly
            //without this being here there are extra items at the end where it hasnt cleared fully before adding new items
            Thread.Sleep(50);

            int counter = 0;
            foreach (var song in scraper.GetSongTitleAndIDs())
            {
                counter++;
                SelectedAlbumSongs.Add(new SongWithNumberAndGuid {Number = counter.ToString(),Title = song.Title,Guid = song.Guid});
                Console.WriteLine("added {0}",song.Title);
            }

            foreach (var row in detailRows)
            {
                row.SongsFromWebsite = SelectedAlbumSongs;
                row.AlbumArtistGuid = new MediaIdGuid {Guid = scraper.ScrapeAlbumArtistID(),MediaId = MediaIds.ZuneAlbumArtistMediaID};
                row.AlbumMediaGuid = new MediaIdGuid {Guid = scraper.ScrapeAlbumMediaID(),MediaId = MediaIds.ZuneAlbumMediaID};
            }
   
            albumDetails.SongCount = counter.ToString();

            this.IsLoading = false;
        }

        public void ViewShown()
        {
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
using System;
using System.Collections.ObjectModel;
using System.Linq;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Models;
using System.Threading;


namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsViewModel : ZuneWizardPageViewModelBase
    {
        private readonly ZuneWizardModel _model;
        private bool _isLoading;
        private SearchResultsDetailsViewModel _searchResultsDetailsViewModel;

        public SearchResultsViewModel(ZuneWizardModel model)
        {
            _model = model;
            this.SearchResultsDetailsViewModel = new SearchResultsDetailsViewModel();
            this.SearchBarViewModel.StartedSearching += SearchBarViewModel_StartedSearching;
        }

        private void SearchBarViewModel_StartedSearching(object sender, EventArgs e)
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
            this.IsLoading = true;

            ThreadPool.QueueUserWorkItem(_ =>
             {
                 try
                 {
                     var reader = new AlbumDocumentReader(url);

                     Album album = reader.Read();

                     //do updating of controls on bound ui objects on UI thread
                     base.UIDispatcher.Invoke(new Action(() =>
                         {
                             if (album.IsValid)
                             {
                                 UpdateAlbumMetaDataViewModel(album);
                                 AddSelectedSongs(album);
                             }
                             else
                             {
                                 this.SearchResultsDetailsViewModel.SelectedAlbumTitle =
                                     "Sorry could not get album details";
                             }}));

                     this.IsLoading = false;
                 }
                 catch (Exception)
                 {
                     this.SearchResultsDetailsViewModel.SelectedAlbumTitle =
                            "Sorry could not get album details";
                 }
             });


        }

        private void UpdateAlbumMetaDataViewModel(Album scrapeResult)
        {
            _model.AlbumDetailsFromWebsite = new WebsiteAlbumMetaDataViewModel
                                                 {
                                                     Title = scrapeResult.AlbumTitle,
                                                     Artist = scrapeResult.AlbumArtist,
                                                     ArtworkUrl = scrapeResult.AlbumArtworkUrl,
                                                     Year = scrapeResult.AlbumReleaseYear.ToString(),
                                                     SongCount = scrapeResult.Tracks.Count().ToString()
                                                 };
        }

        private void AddSelectedSongs(Album album)
        {
            this.SearchResultsDetailsViewModel = new SearchResultsDetailsViewModel { SelectedAlbumTitle = album.AlbumTitle };

            foreach (var track in album.Tracks)
                this.SearchResultsDetailsViewModel.SelectedAlbumSongs.Add(track);

            foreach (var row in _model.Rows)
            {
                row.SongsFromWebsite = this.SearchResultsDetailsViewModel.SelectedAlbumSongs;
                row.AlbumDetails = album;
            }
        }

        internal override bool IsNextEnabled()
        {
            return true;
        }
    }
}
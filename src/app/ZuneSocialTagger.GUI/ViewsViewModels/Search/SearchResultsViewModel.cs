using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Details;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private IEnumerable<WebAlbum> _albums;
        private IEnumerable<WebArtist> _artists;
        private bool _isLoading;
        private SearchResultsDetailViewModel _searchResultsDetailViewModel;
        private string _albumCount;
        private bool _isAlbumsEnabled;
        private string _artistCount;

        internal WebAlbum _downloadedAlbum;

        public SearchResultsViewModel()
        {
            this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel();
            this.SearchResults = new ObservableCollection<object>();
            this.SearchResults.CollectionChanged += SearchResults_CollectionChanged;
  
            this.ArtistCommand = new RelayCommand(DisplayArtists);
            this.AlbumCommand = new RelayCommand(DisplayAlbums);
            this.ResultClickedCommand = new RelayCommand<object>(ResultClicked);

            this.ArtistCount = "";
            this.AlbumCount = "";
        }

        #region Bindings

        public ObservableCollection<object> SearchResults { get; set; }
        public RelayCommand ArtistCommand { get; private set; }
        public RelayCommand AlbumCommand { get; private set; }
        public RelayCommand<object>ResultClickedCommand { get; private set; }

        public SearchResultsDetailViewModel SearchResultsDetailViewModel
        {
            get { return _searchResultsDetailViewModel; }
            set
            {
                _searchResultsDetailViewModel = value;
                RaisePropertyChanged(() => this.SearchResultsDetailViewModel);
            }
        }

        public bool HasResults
        {
            get { return this.SearchResults.Count > 0; }
        }

        public string AlbumCount
        {
            get { return _albumCount; }
            set
            {
                _albumCount = value;
                RaisePropertyChanged(() => this.AlbumCount);
            }
        }

        public string ArtistCount
        {
            get { return _artistCount; }
            set
            {
                _artistCount = value;
                RaisePropertyChanged(() => this.ArtistCount);
            }
        }

        public bool IsAlbumsEnabled
        {
            get { return _isAlbumsEnabled; }
            set
            {
                _isAlbumsEnabled = value;

                if (_isAlbumsEnabled)
                    DisplayAlbums();
                else
                {
                    DisplayArtists();
                    this.SearchResultsDetailViewModel = null;
                }


                RaisePropertyChanged(() => this.IsAlbumsEnabled);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => this.IsLoading);
            }
        }

        #endregion

        public void LoadAlbum(WebAlbum album)
        {
            this.IsLoading = true;

            if (album == null) return;

            string fullUrlToAlbumXmlDetails = String.Concat(Urls.Album, album.AlbumMediaId);

            var reader = new AlbumDetailsDownloader(fullUrlToAlbumXmlDetails);

            reader.DownloadCompleted += (details, state) => {
                if (state == DownloadState.Success)
                {
                    _downloadedAlbum = details;
                    UpdateDetail(details);
                    this.IsLoading = false;
                }
                else
                {
                    this.SearchResultsDetailViewModel.SelectedAlbumTitle =
                        "Could not get album details";

                    this.IsLoading = false;
                }
            };
            
            reader.DownloadAsync();
        }

        public void LoadAlbumsForArtist(WebArtist artist)
        {
            AlbumSearch.SearchForAlbumFromArtistGuidAsync(artist.Id, results =>
            {
                _albums = results.ToList();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    this.SearchResults.Clear();

                    foreach (var album in _albums)
                        this.SearchResults.Add(album);

                    this.AlbumCount = String.Format("ALBUMS ({0})", _albums.Count());

                    this.IsAlbumsEnabled = true;
                });
            });
        }

        private void DisplayArtists()
        {
            this.SearchResults.Clear();

            foreach (var artist in _artists)
                this.SearchResults.Add(artist);

            RaisePropertyChanged(() => this.HasResults);
        }

        private void DisplayAlbums()
        {
            this.SearchResults.Clear();

            foreach (var album in _albums)
                this.SearchResults.Add(album);

            RaisePropertyChanged(() => this.HasResults);
        }

        public void LoadArtists(IEnumerable<WebArtist> artists)
        {
            _artists = artists;
            this.ArtistCount = String.Format("ARTISTS ({0})", artists.Count());
        }

        public void LoadAlbums(IEnumerable<WebAlbum> albums)
        {
            _albums = albums;
            this.AlbumCount = String.Format("ALBUMS ({0})", albums.Count());

            foreach (WebAlbum album in albums)
                this.SearchResults.Add(album);

            this.IsAlbumsEnabled = true;
        }

        //private void MoveBack()
        //{
        //    _locator.SwitchToViewModel<SearchViewModel>();
        //}

        //private void MoveNext()
        //{
        //    var detailsViewModel = _locator.SwitchToViewModel<DetailsViewModel>();
        //    detailsViewModel.AlbumDetailsFromWebsite = _downloadedAlbum.GetAlbumDetailsFrom();
        //    detailsViewModel._tracksFromWeb = _downloadedAlbum.Tracks;
        //    detailsViewModel.PopulateRows();
        //}

        private void UpdateDetail(WebAlbum albumMetaData)
        {
             this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel
                                                    {
                                                        SelectedAlbumTitle = albumMetaData.Title
                                                    };

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (var track in albumMetaData.Tracks)
                {
                    this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(new TrackWithTrackNum
                                                                                 {
                                                                                     TrackNumber = track.TrackNumber,
                                                                                     TrackTitle = track.Title
                                                                                 });
                }

            });
        }

        private void SearchResults_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewStartingIndex == 0 && e.NewItems[0].GetType() == typeof(WebAlbum))
            {
                LoadAlbum((WebAlbum)e.NewItems[0]);
            }
        }

        private void ResultClicked(object item)
        {
            if (item.GetType() == typeof(WebArtist))
                LoadAlbumsForArtist(item as WebArtist);

            if (item.GetType() == typeof(WebAlbum))
                LoadAlbum(item as WebAlbum);
        }
    }
}
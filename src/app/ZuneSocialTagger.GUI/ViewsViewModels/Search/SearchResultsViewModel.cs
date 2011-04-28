using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using System.Diagnostics;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private readonly SearchViewModel _parent;
        private IEnumerable<WebAlbum> _albums;
        private IEnumerable<WebArtist> _artists;

        public SearchResultsViewModel(SearchViewModel parent)
        {
            _parent = parent;
            SelectedAlbumSongs = new ObservableCollection<TrackWithTrackNum>();
        }

        public WebAlbum DownloadedAlbum { get; private set; }

        #region Bindings

        public ObservableCollection<TrackWithTrackNum> SelectedAlbumSongs { get; set; }

        private string _selectedAlbumTitle;
        public string SelectedAlbumTitle
        {
            get { return _selectedAlbumTitle; }
            set
            {
                _selectedAlbumTitle = value;
                RaisePropertyChanged(() => SelectedAlbumTitle);
            }
        }

        private ObservableCollection<object> _searchResults;
        public ObservableCollection<object> SearchResults 
        {
            get
            {
                if (_searchResults == null)
                {
                    _searchResults = new ObservableCollection<object>();
                    _searchResults.CollectionChanged += SearchResults_CollectionChanged;
                }

                return _searchResults;
            }
        }

        private RelayCommand _artistCommand;
        public RelayCommand ArtistCommand
        {
            get
            {
                if (_artistCommand == null)
                    _artistCommand = new RelayCommand(DisplayArtists);

                return _artistCommand;
            }
        }

        private RelayCommand _albumCommand;
        public RelayCommand AlbumCommand
        {
            get
            {
                if (_albumCommand == null)
                    _albumCommand = new RelayCommand(DisplayAlbums);

                return _albumCommand;
            }
        }

        private RelayCommand<object> _resultClickedCommand;
        public RelayCommand<object> ResultClickedCommand 
        {
            get 
            {
                if (_resultClickedCommand == null)
                    _resultClickedCommand = new RelayCommand<object>(ResultClicked);

                return _resultClickedCommand;
            }
        }

        public bool HasResults
        {
            get { return this.SearchResults.Count > 0; }
        }

        private string _albumCount;
        public string AlbumCount
        {
            get { return _albumCount; }
            set
            {
                _albumCount = value;
                RaisePropertyChanged(() => this.AlbumCount);
            }
        }

        private int _resultsWidth;
        public int ResultsWidth
        {
            get { return _resultsWidth; }
            set
            {
                _resultsWidth = value;
                RaisePropertyChanged(() => this.ResultsWidth);
            }
        }

        private string _artistCount;
        public string ArtistCount
        {
            get { return _artistCount; }
            set
            {
                _artistCount = value;
                RaisePropertyChanged(() => this.ArtistCount);
            }
        }

        private bool _isAlbumsEnabled;
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
                }

                RaisePropertyChanged(() => this.IsAlbumsEnabled);
            }
        }

        private bool _isLoading;
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
            _parent.CanMoveNext = false;
            this.IsLoading = true;

            if (album == null) return;

            string fullUrlToAlbumXmlDetails = String.Concat(Urls.Album, album.AlbumMediaId);

            AlbumDetailsDownloader.DownloadAsync(fullUrlToAlbumXmlDetails, (webAlbum)=>
            {
                if (webAlbum != null)
                {
                    DownloadedAlbum = webAlbum;
                    UpdateDetail(DownloadedAlbum);
                }
                else
                {
                    SelectedAlbumTitle =
                        "Could not get album details";
                }

                _parent.CanMoveNext = true;
                this.IsLoading = false;
            });
        }

        public void LoadAlbumsForArtist(WebArtist artist)
        {
            _parent.IsSearching = true;
            AlbumSearch.SearchForAlbumFromArtistGuidAsync(artist.Id, results =>
            {
                _albums = results.ToList();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    _parent.IsSearching = false;
                    this.SearchResults.Clear();

                    foreach (var album in _albums)
                        this.SearchResults.Add(album);

                    this.AlbumCount = String.Format("ALBUMS ({0})", _albums.Count());

                    this.IsAlbumsEnabled = true;
                });
            });
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

        private void DisplayArtists()
        {
            this.ResultsWidth = 640;
            _parent.CanMoveNext = false;
            this.SearchResults.Clear();

            foreach (var artist in _artists)
                this.SearchResults.Add(artist);

            RaisePropertyChanged(() => this.HasResults);
        }

        private void DisplayAlbums()
        {
            this.ResultsWidth = 300;
            this.SearchResults.Clear();

            foreach (var album in _albums)
                this.SearchResults.Add(album);

            RaisePropertyChanged(() => this.HasResults);
        }

        private void UpdateDetail(WebAlbum albumMetaData)
        {
            SelectedAlbumTitle = albumMetaData.Title;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                SelectedAlbumSongs.Clear();
                foreach (var track in albumMetaData.Tracks)
                {
                    var tnum = new TrackWithTrackNum();
                    tnum.TrackNumber = track.TrackNumber;
                    tnum.TrackTitle = track.Title;

                   SelectedAlbumSongs.Add(tnum);
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
            if (item != null)
            {
                if (item.GetType() == typeof(WebArtist))
                    LoadAlbumsForArtist(item as WebArtist);

                if (item.GetType() == typeof(WebAlbum))
                    LoadAlbum(item as WebAlbum);
            }
        }
    }
}
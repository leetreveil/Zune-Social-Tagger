using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using System.Threading;
using Album = ZuneSocialTagger.Core.ZuneWebsite.Album;
using AlbumDocumentReader = ZuneSocialTagger.Core.ZuneWebsite.AlbumDocumentReader;
using System.Windows.Threading;


namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SearchResultsViewModel : ViewModelBaseExtended
    {
        private readonly ZuneWizardModel _model;
        private readonly Dispatcher _dispatcher;
        private IEnumerable<Album> _albums;
        private IEnumerable<Artist> _artists;
        private bool _isLoading;
        private SearchResultsDetailViewModel _searchResultsDetailViewModel;
        private string _albumCount;
        private bool _isAlbumsEnabled;
        private string _artistCount;

        public SearchResultsViewModel(ZuneWizardModel model, Dispatcher dispatcher,
                                      IEnumerable<Album> albums)
        {
            _model = model;
            _dispatcher = dispatcher;
            _albums = albums;

            this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel();
            this.SearchResults = new ObservableCollection<object>();
  
            this.MoveNextCommand = new RelayCommand(MoveNext);
            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.ArtistCommand = new RelayCommand(DisplayArtists);
            this.AlbumCommand = new RelayCommand(DisplayAlbums);

            this.ArtistCount = "";
            this.AlbumCount = "";

            foreach (Album album in albums)
                this.SearchResults.Add(album);

            this.AlbumCount = String.Format("ALBUMS ({0})", albums.Count());
            this.IsAlbumsEnabled = true;
        }

        public ObservableCollection<object> SearchResults { get; set; }
        public RelayCommand MoveNextCommand { get; private set; }
        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand ArtistCommand { get; private set; }
        public RelayCommand AlbumCommand { get; private set; }

        public SearchResultsDetailViewModel SearchResultsDetailViewModel
        {
            get { return _searchResultsDetailViewModel; }
            set
            {
                    _searchResultsDetailViewModel = value;
                   RaisePropertyChanged(() => this.SearchResultsDetailViewModel);
            }
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

        public void LoadAlbum(Album album)
        {
            this.IsLoading = true;

            if (album == null) return;

            string fullUrlToAlbumXmlDetails = String.Concat(Urls.Album, album.AlbumMediaID);

            ThreadPool.QueueUserWorkItem(_ =>
             {
                 try
                 {
                     var reader = new AlbumDocumentReader(fullUrlToAlbumXmlDetails);

                     IEnumerable<Track> tracks = reader.Read();

                     _model.SelectedAlbum.WebAlbumMetaData =  SetAlbumDetails(tracks);
                     _model.SelectedAlbum.SongsFromWebsite = tracks.ToObservableCollection();

                     //Set each row's selected song based upon whats available from the zune website
                     foreach (var row in _model.SelectedAlbum.Tracks)
                     {
                         row.SelectedSong = row.MatchThisSongToAvailableSongs(_model.SelectedAlbum.SongsFromWebsite);
                     }

                     UpdateDetail(tracks);

                     this.IsLoading = false;
                 }
                 catch (Exception)
                 {
                     this.SearchResultsDetailViewModel.SelectedAlbumTitle =
                            "Could not get album details";

                     this.IsLoading = false;
                 }
             });
        }

        public void LoadAlbumsForArtist(Artist artist)
        {
            ThreadPool.QueueUserWorkItem(_ => {
                IEnumerable<Album> albums = AlbumSearch.GetAlbumsFromArtistGuid(artist.Id).ToList();

                _albums = albums;

                _dispatcher.Invoke(new Action(delegate {

                    this.SearchResults.Clear();

                    foreach (var album in _albums)
                        this.SearchResults.Add(album);

                    this.AlbumCount = String.Format("ALBUMS ({0})", albums.Count());

                    this.IsAlbumsEnabled = true;
                }));
            });
        }

        private void DisplayArtists()
        {
            this.SearchResults.Clear();

            foreach (var artist in _artists)
                this.SearchResults.Add(artist);
        }

        private void DisplayAlbums()
        {
            this.SearchResults.Clear();

            foreach (var album in _albums)
                this.SearchResults.Add(album);
        }

        public void LoadArtists(IEnumerable<Artist> artists)
        {
            _artists = artists;
            this.ArtistCount = String.Format("ARTISTS ({0})", artists.Count());
        }

        private void MoveBack()
        {
            Messenger.Default.Send(typeof(SearchViewModel));
        }

        private void MoveNext()
        {
            Messenger.Default.Send(typeof(DetailsViewModel));
        }


        private static ExpandedAlbumDetailsViewModel SetAlbumDetails(IEnumerable<Track> tracks)
        {
            MetaData firstTracksMetaData = tracks.First().MetaData;

            return new ExpandedAlbumDetailsViewModel
                 {
                     Title = firstTracksMetaData.AlbumName,
                     Artist = firstTracksMetaData.AlbumArtist,
                     ArtworkUrl = tracks.First().ArtworkUrl,
                     Year = firstTracksMetaData.Year,
                     SongCount = tracks.Count().ToString()
                 };
        }

        private void UpdateDetail(IEnumerable<Track> tracks)
        {
             this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel
                                                    {
                                                        SelectedAlbumTitle = tracks.First().MetaData.AlbumName
                                                    };

            _dispatcher.Invoke(new Action(() =>
                 {
                     foreach (var track in tracks)
                         this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track);
                 }));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Models;
using System.Threading;
using Album = ZuneSocialTagger.Core.Album;
using AlbumDocumentReader = ZuneSocialTagger.Core.ZuneWebsite.AlbumDocumentReader;


namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private readonly IZuneWizardModel _model;
        private bool _isLoading;
        private SearchResultsDetailViewModel _searchResultsDetailViewModel;
        private ExpandedAlbumDetailsViewModel _albumDetails;

        public SearchResultsViewModel(IZuneWizardModel model, SearchHeaderViewModel searchHeaderViewModel)
        {
            _model = model;

            this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel();
            this.SearchHeader = searchHeaderViewModel;
            this.Albums = _model.FoundAlbums;

            this.MoveNextCommand = new RelayCommand(MoveNext);
            this.MoveBackCommand = new RelayCommand(MoveBack);
        }

        public ObservableCollection<Album> Albums { get; set; }
        public RelayCommand MoveNextCommand { get; private set; }
        public RelayCommand MoveBackCommand { get; private set; }

        public SearchResultsDetailViewModel SearchResultsDetailViewModel
        {
            get { return _searchResultsDetailViewModel; }
            set
            {
                    _searchResultsDetailViewModel = value;
                   RaisePropertyChanged("SearchResultsDetailViewModel");
            }
        }

        public string AlbumCount
        {
            get { return String.Format("ALBUMS ({0})", this.Albums.Count); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public SearchHeaderViewModel SearchHeader { get; set; }

        public void MoveBack()
        {
            Messenger.Default.Send(typeof(SearchViewModel));
        }

        public void MoveNext()
        {
            Messenger.Default.Send(typeof(DetailsViewModel));
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

                     SetAlbumDetails(tracks);
                     AddSelectedSongs(tracks);


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

        private ExpandedAlbumDetailsViewModel SetAlbumDetails(IEnumerable<Track> tracks)
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

        private void AddSelectedSongs(IEnumerable<Track> tracks)
        {
            Dispatcher dispatcher = UIDispatcher.GetDispatcher();

            this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel
                                                    {
                                                        SelectedAlbumTitle = tracks.First().MetaData.AlbumName
                              
                                                    };


            dispatcher.Invoke(new Action(() =>
                 {
                     foreach (var track in tracks)
                         this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track);
                 }));


            foreach (var row in _model.Rows)
            {
                row.SongsFromWebsite = this.SearchResultsDetailViewModel.SelectedAlbumSongs;
                row.Tracks = tracks;
            }
        }
    }
}
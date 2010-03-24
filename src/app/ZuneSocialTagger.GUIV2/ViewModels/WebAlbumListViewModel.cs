using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using System.Threading;
using Album = ZuneSocialTagger.Core.ZuneDatabase.Album;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;
using System.Diagnostics;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WebAlbumListViewModel : ViewModelBase, IFirstPage
    {
        private IZuneDbAdapter _dbAdapter;
        private readonly IZuneWizardModel _model;
        private bool _isLoading;
        private int _loadingProgress;
        private string _scanAllText;
        private bool _isDownloadingAlbumDetails;
        private AsyncObservableCollection<AlbumDetailsViewModel> _albums;
        private AlbumDownloaderWithProgressReporting _downloader;

        public WebAlbumListViewModel(IZuneDbAdapter dbAdapter,IZuneWizardModel model)
        {
            _dbAdapter = dbAdapter;
            _model = model;
            _dbAdapter.FinishedReadingAlbums += DbAdapterFinishedReadingAlbums;
            _dbAdapter.ProgressChanged += DbAdapterProgressChanged;

            this.Albums = new AsyncObservableCollection<AlbumDetailsViewModel>();

            this.SortViewModel = new SortViewModel();
            this.SortViewModel.SortClicked += SortViewModel_SortClicked;

            this.ScanAllText = "SCAN ALL";

            SetupCommandBindings();

            ReadDatabase();
        }

        #region View Binding Properties

        public AsyncObservableCollection<AlbumDetailsViewModel> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                RaisePropertyChanged("Albums");
            }
        }

        public SortViewModel SortViewModel { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public bool IsDownloadingAlbumDetails
        {
            get { return _isDownloadingAlbumDetails; }
            set
            {
                _isDownloadingAlbumDetails = value;
                this.ScanAllText = IsDownloadingAlbumDetails ? "STOP" : "SCAN ALL";
                RaisePropertyChanged("IsDownloadingAlbumDetails");
            }
        }

        public int LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                RaisePropertyChanged("LoadingProgress");
            }
        }

        public int LinkedTotal
        {
            get { return this.Albums.Where(x => x.LinkStatus == LinkStatus.Linked).Count(); }
        }

        public int UnlinkedTotal
        {
            get
            {
                return this.Albums.Where(x => x.LinkStatus == LinkStatus.Unlinked
                                              || x.LinkStatus == LinkStatus.Unavailable).Count();
            }
        }

        public int AlbumOrArtistMismatchTotal
        {
            get { return this.Albums.Where(x => x.LinkStatus == LinkStatus.AlbumOrArtistMismatch).Count(); }
        }

        public string ScanAllText
        {
            get { return _scanAllText; }
            set
            {
                _scanAllText = value;
                RaisePropertyChanged("ScanAllText");
            }
        }

        public RelayCommand LoadDatabaseCommand { get; private set; }
        public RelayCommand LoadFromZuneWebsiteCommand { get; private set; }
        public RelayCommand CancelDownloadingCommand { get; private set; }
        public RelayCommand SwitchToClassicModeCommand { get; private set; }

        #endregion

        public void LoadFromZuneWebsite()
        {
            if (this.IsDownloadingAlbumDetails)
                CancelDownloading();
            else
            {
                this.IsDownloadingAlbumDetails = true;

                _downloader = new AlbumDownloaderWithProgressReporting(
                    this.Albums.Where(x=> x.WebAlbumMetaData == null && x.LinkStatus != LinkStatus.Unlinked));

                _downloader.ProgressChanged += downloader_ProgressChanged;
                _downloader.FinishedDownloadingAlbums += downloader_FinishedDownloadingAlbums;
                _downloader.Start();
            }
        }

        public void CancelDownloading()
        {
            if (_downloader != null)
                _downloader.StopDownloading();
        }

        public void RefreshDatabase()
        {
            this.IsLoading = true;
            this.Albums.Clear();
            this.SortViewModel.SortOrder = SortOrder.NotSorted;

            if (_dbAdapter.GetType() == typeof(CachedZuneDatabaseReader))
            {
                //if we are using the cache then we want to switch to the real database to refresh
                //send message to the app model and tell it to switch the database
                Messenger.Default.Send<string>("SWITCHTODB");
            }
            else
            {
                ReadDatabase();
            }
        }

        private bool ReadDatabase()
        {
            return ThreadPool.QueueUserWorkItem(delegate
            {
                foreach (AlbumDetails newAlbum in _dbAdapter.ReadAlbums())
                {
                    Debug.WriteLine(newAlbum.ZuneAlbumMetaData.AlbumTitle);
                    //add handler to be notified when the LinkStatus enum changes
                    //newAlbum.PropertyChanged += album_PropertyChanged;

                    if (newAlbum.ZuneAlbumMetaData.AlbumMediaId == Guid.Empty)
                        newAlbum.LinkStatus = LinkStatus.Unlinked;

                    this.Albums.Add(new AlbumDetailsViewModel(newAlbum));
                }

                this.IsLoading = false;
            });
        }

        public void LinkAlbum(Album albumDetails)
        {
            if (!DoesAlbumExistInDbAndDisplayError(albumDetails)) return;

            IEnumerable<Track> tracksForAlbum = _dbAdapter.GetTracksForAlbum(albumDetails.MediaId);

            _model.Rows = new ObservableCollection<DetailRow>();

            foreach (var track in tracksForAlbum)
            {
                try
                {
                    //TODO: do we really want to not be able to link an album if just one track cant be read?
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(track.FilePath);

                    _model.Rows.Add(new DetailRow(track.FilePath, container));
                }
                catch (AudioFileReadException ex)
                {
                    ZuneMessageBox.Show(ex.Message, ErrorMode.Error);
                    return;
                }
            }

            _model.SearchText = albumDetails.AlbumArtist + " " + albumDetails.AlbumTitle;

            _model.FileAlbumDetails = new ExpandedAlbumDetailsViewModel
            {
                Artist = albumDetails.AlbumArtist,
                Title = albumDetails.AlbumTitle,
                ArtworkUrl = albumDetails.ArtworkUrl,
                SongCount = albumDetails.TrackCount.ToString(),
                Year = albumDetails.ReleaseYear.ToString()
            };

            //tell the application to switch to the search view
            Messenger.Default.Send(typeof(SearchViewModel));
        }

        public void DelinkAlbum(Album albumDetails)
        {
            if (!DoesAlbumExistInDbAndDisplayError(albumDetails)) return;

            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            var tracksForAlbum = _dbAdapter.GetTracksForAlbum(albumDetails.MediaId).ToList();

            //_dbReader.RemoveAlbumFromDatabase(this.ZuneAlbumMetaData.MediaId);

            foreach (var track in tracksForAlbum)
            {
                try
                {
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(track.FilePath);

                    _model.Rows.Add(new DetailRow(track.FilePath, container));

                    container.RemoveZuneAttribute("WM/WMContentID");
                    container.RemoveZuneAttribute("WM/WMCollectionID");
                    container.RemoveZuneAttribute("WM/WMCollectionGroupID");
                    container.RemoveZuneAttribute("ZuneCollectionID");
                    container.RemoveZuneAttribute("WM/UniqueFileIdentifier");
                    container.RemoveZuneAttribute("ZuneCollectionID");
                    container.RemoveZuneAttribute("ZuneUserEditedFields");
                    container.RemoveZuneAttribute(ZuneIds.Album);
                    container.RemoveZuneAttribute(ZuneIds.Artist);
                    container.RemoveZuneAttribute(ZuneIds.Track);

                    container.WriteToFile(track.FilePath);
                }
                catch (AudioFileReadException ex)
                {
                    ZuneMessageBox.Show(ex.Message, ErrorMode.Error);
                    return;
                }

            }

            //foreach (var track in tracksForAlbum)
            //    _dbReader.AddTrackToDatabase(track.FilePath);

            Mouse.OverrideCursor = null;

            //TODO: change this to an information message box because it is not an error
            ZuneMessageBox.Show("Album should now be de-linked. You may need to " +
                                "remove then re-add the album for the changes to take effect.", ErrorMode.Warning);
        }

        public void RefreshAlbum(AlbumDetailsViewModel albumDetails)
        {
            if (!DoesAlbumExistInDbAndDisplayError(albumDetails.ZuneAlbumMetaData)) return;

            ThreadPool.QueueUserWorkItem(_ =>
             {
                 Album albumMetaData = _dbAdapter.GetAlbum(albumDetails.ZuneAlbumMetaData.MediaId).ZuneAlbumMetaData;

                 albumDetails.LinkStatus = LinkStatus.Unknown;

                 string url = String.Concat(Urls.Album, albumMetaData.AlbumMediaId);

                 if (albumMetaData.AlbumMediaId != Guid.Empty)
                 {
                     var downloader = new AlbumDetailsDownloader(url);

                     downloader.DownloadCompleted += (alb,state) =>
                         {
                             albumDetails.LinkStatus = SharedMethods.GetAlbumLinkStatus(alb.AlbumTitle,
                                                                                alb.AlbumArtist,
                                                                                albumMetaData.AlbumTitle,
                                                                                albumMetaData.AlbumArtist);

                             albumDetails.WebAlbumMetaData = new Album
                                                         {
                                                             AlbumArtist = alb.AlbumArtist,
                                                             AlbumTitle = alb.AlbumTitle,
                                                             ArtworkUrl = alb.ArtworkUrl
                                                         };
                         };

                     downloader.DownloadAsync();
                 }
                 else
                 {
                     albumDetails.LinkStatus = LinkStatus.Unlinked;
                 }
             });
        }

        private bool DoesAlbumExistInDbAndDisplayError(Album selectedAlbum)
        {
            bool doesAlbumExist = _dbAdapter.DoesAlbumExist(selectedAlbum.MediaId);

            if (!doesAlbumExist)
            {
                SharedMethods.ShowCouldNotFindAlbumError();
                return false;
            }

            return true;
        }

        public void SwitchToClassicMode()
        {
            Messenger.Default.Send(typeof(SelectAudioFilesViewModel));
        }

        private void SetupCommandBindings()
        {
            this.LoadDatabaseCommand = new RelayCommand(RefreshDatabase);
            this.LoadFromZuneWebsiteCommand = new RelayCommand(LoadFromZuneWebsite);
            this.CancelDownloadingCommand = new RelayCommand(CancelDownloading);
            this.SwitchToClassicModeCommand = new RelayCommand(SwitchToClassicMode);
        }

        private void PerformSort(SortOrder sortOrder)
        {
            ThreadPool.QueueUserWorkItem(_ =>
                 {
                     //TODO: remove code repetition
                     switch (sortOrder)
                     {
                         case SortOrder.DateAdded:
                             DoSort(x=> x.ZuneAlbumMetaData.DateAdded);
                             break;
                         case SortOrder.Album:
                             DoSort(x => x.ZuneAlbumMetaData.AlbumTitle);
                             break;
                         case SortOrder.Artist:
                             DoSort(x => x.ZuneAlbumMetaData.AlbumArtist);
                             break;
                         case SortOrder.LinkStatus:
                             DoSort(x => x.LinkStatus);
                             break;
                         default:
                             throw new ArgumentOutOfRangeException();
                     }
                 });
        }

        private void DoSort<T>(Func<AlbumDetailsViewModel,T> sortKey)
        {
            this.Albums = this.Albums.OrderBy(sortKey).ToAsyncObservableCollection();
        }

        private void SortViewModel_SortClicked(SortOrder sortOrder)
        {
            PerformSort(sortOrder);
        }

        private void album_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LinkStatus")
                UpdateLinkTotals();
        }

        private void DbAdapterProgressChanged(int arg1, int arg2)
        {
            UpdateLinkTotals();
            ReportProgress(arg1, arg2);
        }

        private void DbAdapterFinishedReadingAlbums()
        {
            ResetLoadingProgress();
            PerformSort(SortOrder.DateAdded);
            this.SortViewModel.Sort(SortOrder.DateAdded);
        }

        private void ReportProgress(int current, int total)
        {
            this.LoadingProgress = current*100/total;
        }

        private void downloader_FinishedDownloadingAlbums()
        {
            ResetLoadingProgress();
        }

        private void downloader_ProgressChanged(int arg1, int arg2)
        {
            //arg1 = current album ;arg2 = total albums
            if (arg1 < arg2)
                ReportProgress(arg1, arg2);

            UpdateLinkTotals();
        }

        private void ResetLoadingProgress()
        {
            this.LoadingProgress = 0;
            this.IsDownloadingAlbumDetails = false;
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged("LinkedTotal");
            this.RaisePropertyChanged("UnlinkedTotal");
            this.RaisePropertyChanged("AlbumOrArtistMismatchTotal");
        }
    }
}
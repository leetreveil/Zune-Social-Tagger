using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Taskbar;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Controls;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using System.Threading;
using ZuneSocialTagger.GUIV2.Properties;
using Album = ZuneSocialTagger.Core.ZuneDatabase.Album;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;
using ZuneSocialTagger.Core.ZuneDatabase;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WebAlbumListViewModel : ViewModelBase, IFirstPage
    {
        private readonly IZuneWizardModel _model;
        private readonly IZuneDatabaseReader _dbReader;
        private bool _canShowScanAllButton;
        private int _loadingProgress;
        private string _scanAllText;
        private AlbumDownloaderWithProgressReporting _downloader;
        private bool _canShowProgressBar;
        private bool _canShowReloadButton;
        private readonly bool _isTaskbarSupported;
        private SortOrder _sortOrder;

        public WebAlbumListViewModel(IZuneWizardModel model, IZuneDatabaseReader dbReader,CachedZuneDatabaseReader cacheReader)
        {
            _model = model;
            this.Albums = _model.AlbumsFromDatabase;

            _dbReader = dbReader;
            _dbReader.FinishedReadingAlbums += DbAdapterFinishedReadingAlbums;
            _dbReader.ProgressChanged += DbAdapterProgressChanged;
            _dbReader.StartedReadingAlbums += _dbAdapter_StartedReadingAlbums;

            cacheReader.FinishedReadingAlbums += cacheReader_FinishedReadingAlbums;

            this.ScanAllText = "SCAN ALL";
            _isTaskbarSupported = TaskbarManager.IsPlatformSupported;

            SetupCommandBindings();

            this.CanShowReloadButton = true;
            this.CanShowScanAllButton = true;

            this.SortOrder = Settings.Default.SortOrder;

            if (_model.SelectedAlbum != null && _model.SelectedAlbum.AlbumDetails.NeedsRefreshing)
            {
                RefreshAlbum(_model.SelectedAlbum.AlbumDetails);
                UpdateLinkTotals();
                _model.SelectedAlbum.AlbumDetails.NeedsRefreshing = false;
            }
        }

        private void SetupCommandBindings()
        {
            this.LoadDatabaseCommand = new RelayCommand(RefreshDatabase);
            this.LoadFromZuneWebsiteCommand = new RelayCommand(LoadFromZuneWebsite);
            this.CancelDownloadingCommand = new RelayCommand(CancelDownloading);
            this.SwitchToClassicModeCommand = new RelayCommand(SwitchToClassicMode);
        }

        #region View Binding Properties

        public bool IsCurrentAlbumLinkable
        {
            get
            {
                return true;
            }
        }

        public int SelectedIndex
        {
            get { return _model.SelectedItemInListView; }
            set { _model.SelectedItemInListView = value; }
        }

        public ObservableCollection<AlbumDetailsViewModel> Albums
        {
            get { return _model.AlbumsFromDatabase; }
            set
            {
                _model.AlbumsFromDatabase = value;
                RaisePropertyChanged("Albums");
            }
        }

        public bool CanShowProgressBar
        {
            get { return _canShowProgressBar; }
            set
            {
                _canShowProgressBar = value;
                this.ScanAllText = CanShowProgressBar ? "STOP" : "SCAN ALL";
                RaisePropertyChanged("CanShowProgressBar");
            }
        }

        public bool CanShowReloadButton
        {
            get { return _canShowReloadButton; }
            set
            {
                _canShowReloadButton = value;
                RaisePropertyChanged("CanShowReloadButton");
            }
        }

        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; RaisePropertyChanged("SortOrder"); }
        }

        public bool CanShowScanAllButton
        {
            get { return _canShowScanAllButton; }
            set
            {
                _canShowScanAllButton = value;
                RaisePropertyChanged("CanShowScanAllButton");
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
        public RelayCommand<bool> FilterGreenCommand { get; private set; }

        #endregion

        public void LoadFromZuneWebsite()
        {
            if (this.CanShowProgressBar)
                CancelDownloading();
            else
            {
                this.CanShowReloadButton = false;
                this.CanShowProgressBar = true;

                //check if we have already downloaded all the albums
                if (this.Albums.Where(x => x.LinkStatus != LinkStatus.Unlinked).All(x => x.WebAlbumMetaData != null))
                {
                    foreach (var album in this.Albums)
                    {
                        if (album.LinkStatus != LinkStatus.Unlinked)
                        {
                            album.WebAlbumMetaData = null;
                            album.LinkStatus = LinkStatus.Unknown;
                        }
                    }
                    UpdateLinkTotals();
                }

                _downloader = new AlbumDownloaderWithProgressReporting(
                    this.Albums.Where(x => x.WebAlbumMetaData == null && x.LinkStatus != LinkStatus.Unlinked));

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
            string msg = "Are you sure? All downloaded album details will be reset and the database will be reloaded.";

            ZuneMessageBox.Show(new ErrorMessage(ErrorMode.Warning, msg), () =>
                 {
                     this.Albums.Clear();
                     this.SortOrder =SortOrder. NotSorted;
                     Messenger.Default.Send("SWITCHTODB");
                 });
        }

        public void LinkAlbum(Album albumDetails)
        {
            var selectedAlbum = new SelectedAlbum();

            if (!DoesAlbumExistInDbAndDisplayError(albumDetails)) return;

            IEnumerable<Track> tracksForAlbum = _dbReader.GetTracksForAlbum(albumDetails.MediaId);

            var containers = GetFileNamesAndContainers(tracksForAlbum);

            foreach (var item in containers)
                selectedAlbum.Tracks.Add(new Song(item.Key, item.Value));

            if (containers.Count > 0)
            {
                _model.SearchText = albumDetails.AlbumArtist + " " + albumDetails.AlbumTitle;

                selectedAlbum.ZuneAlbumMetaData = new ExpandedAlbumDetailsViewModel
                                              {
                                                  Artist = albumDetails.AlbumArtist,
                                                  Title = albumDetails.AlbumTitle,
                                                  ArtworkUrl = albumDetails.ArtworkUrl,
                                                  SongCount = albumDetails.TrackCount.ToString(),
                                                  Year = albumDetails.ReleaseYear.ToString()
                                              };

                selectedAlbum.AlbumDetails = this.Albums.Where(x => x.ZuneAlbumMetaData == albumDetails).First();

                _model.SelectedAlbum = selectedAlbum;

                //tell the application to switch to the search view
                Messenger.Default.Send(typeof (SearchViewModel));
            }
        }

        public void DelinkAlbum(Album albumDetails)
        {
            if (!DoesAlbumExistInDbAndDisplayError(albumDetails)) return;

            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            var tracksForAlbum = _dbReader.GetTracksForAlbum(albumDetails.MediaId).ToList();

            //_dbReader.RemoveAlbumFromDatabase(this.ZuneAlbumMetaData.MediaId);

            //make sure we can actually read all the files before doing anything to them
            var containers = GetFileNamesAndContainers(tracksForAlbum);

            foreach (var item in containers)
            {
                item.Value.RemoveZuneAttribute("WM/WMContentID");
                item.Value.RemoveZuneAttribute("WM/WMCollectionID");
                item.Value.RemoveZuneAttribute("WM/WMCollectionGroupID");
                item.Value.RemoveZuneAttribute("ZuneCollectionID");
                item.Value.RemoveZuneAttribute("WM/UniqueFileIdentifier");
                item.Value.RemoveZuneAttribute("ZuneCollectionID");
                item.Value.RemoveZuneAttribute("ZuneUserEditedFields");
                item.Value.RemoveZuneAttribute(ZuneIds.Album);
                item.Value.RemoveZuneAttribute(ZuneIds.Artist);
                item.Value.RemoveZuneAttribute(ZuneIds.Track);

                item.Value.WriteToFile(item.Key);
            }

            //foreach (var track in tracksForAlbum)
            //    _dbReader.AddTrackToDatabase(track.FilePath);

            Mouse.OverrideCursor = null;

            if (containers.Count > 0)
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Warning,
                                                        "Album should now be de-linked. You may need to " +
                                                        "remove then re-add the album for the changes to take effect."));

            //force a refresh on the album to see if the de-link worked
            //this probably wont work because the zunedatabase does not correctly change the albums
            //details when delinking, but does when linking
            RefreshAlbum(this.Albums.Where(x => x.ZuneAlbumMetaData == albumDetails).First());
        }

        public void RefreshAlbum(AlbumDetailsViewModel albumDetails)
        {
            if (!DoesAlbumExistInDbAndDisplayError(albumDetails.ZuneAlbumMetaData)) return;

            ThreadPool.QueueUserWorkItem(_ =>
             {
                 Album albumMetaData = _dbReader.GetAlbum(albumDetails.ZuneAlbumMetaData.MediaId);

                 albumDetails.LinkStatus = LinkStatus.Unknown;

                 if (albumMetaData.AlbumMediaId != Guid.Empty)
                 {
                     var downloader = new AlbumDetailsDownloader(String.Concat(Urls.Album, albumMetaData.AlbumMediaId));

                     downloader.DownloadCompleted += (alb, state) =>
                         {
                             if (alb != null)
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
                             }
                             else
                             {
                                 albumDetails.LinkStatus = LinkStatus.Unavailable;
                             }
                         };

                     downloader.DownloadAsync();
                 }
                 else
                 {
                     albumDetails.LinkStatus = LinkStatus.Unlinked;
                 }
             });
        }

        public void SortData(SortOrder sortOrder)
        {
            PerformSort(sortOrder);
        }

        private static Dictionary<string, IZuneTagContainer> GetFileNamesAndContainers(IEnumerable<Track> tracks)
        {
            var albumContainers = new Dictionary<string, IZuneTagContainer>();

            foreach (var track in tracks)
            {
                try
                {
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(track.FilePath);
                    albumContainers.Add(track.FilePath, container);
                }
                catch (Exception ex)
                {
                    Messenger.Default.Send(new ErrorMessage(ErrorMode.Error, ex.Message));
                    break;
                }
            }

            return albumContainers;
        }

        private bool DoesAlbumExistInDbAndDisplayError(Album selectedAlbum)
        {
            bool doesAlbumExist = _dbReader.DoesAlbumExist(selectedAlbum.MediaId);

            if (!doesAlbumExist)
            {
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error, "Could not find album, you may need to refresh the database."));
                return false;
            }

            return true;
        }

        public void SwitchToClassicMode()
        {
            Messenger.Default.Send(typeof(IFirstPage));
        }

        private void PerformSort(SortOrder sortOrder)
        {
            ThreadPool.QueueUserWorkItem(_ =>{
             switch (sortOrder)
             {
                 case SortOrder.DateAdded:
                     DoSort(x => x.ZuneAlbumMetaData.DateAdded);
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
             }});
        }

        private void DoSort<T>(Func<AlbumDetailsViewModel, T> sortKey)
        {
            //we want to order by descending if we use DateTime because we want the newest at the top
            //all the other states we want ascending
            this.Albums = typeof (T) == typeof (DateTime)
                              ? this.Albums.OrderByDescending(sortKey).ToObservableCollection()
                              : this.Albums.OrderBy(sortKey).ToObservableCollection();
        }

        private void _dbAdapter_StartedReadingAlbums()
        {
            this.CanShowReloadButton = false;
            this.CanShowProgressBar = true;
            this.CanShowScanAllButton = false;
        }

        private void DbAdapterProgressChanged(int arg1, int arg2)
        {
            UpdateLinkTotals();
            ReportProgress(arg1, arg2);
        }

        private void DbAdapterFinishedReadingAlbums()
        {
            this.CanShowReloadButton = true;
            this.CanShowProgressBar = false;
            this.CanShowScanAllButton = true;
            ResetLoadingProgress();

            this.SortOrder = Settings.Default.SortOrder;
            PerformSort(Settings.Default.SortOrder);
        }

        private void cacheReader_FinishedReadingAlbums()
        {
            UpdateLinkTotals();
            this.SortOrder = Settings.Default.SortOrder;
            PerformSort(Settings.Default.SortOrder);
            ResetLoadingProgress();
        }

        private void ReportProgress(int current, int total)
        {
            this.LoadingProgress = current*100/total;

            if (_isTaskbarSupported)
                TaskbarManager.Instance.SetProgressValue(current, total);
        }

        private void downloader_FinishedDownloadingAlbums()
        {
            this.CanShowProgressBar = false;
            this.CanShowReloadButton = true;
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

            if (_isTaskbarSupported)
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged("LinkedTotal");
            this.RaisePropertyChanged("UnlinkedTotal");
            this.RaisePropertyChanged("AlbumOrArtistMismatchTotal");
        }
    }
}
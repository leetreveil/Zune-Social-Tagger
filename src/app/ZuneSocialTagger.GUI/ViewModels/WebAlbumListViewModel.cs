using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Taskbar;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Models;
using System.Linq;
using System.Threading;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.Core.ZuneDatabase;

namespace ZuneSocialTagger.GUI.ViewModels
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

        public WebAlbumListViewModel(IZuneWizardModel model, IZuneDatabaseReader dbReader)
        {
            _model = model;
            this.Albums = _model.AlbumsFromDatabase;

            _dbReader = dbReader;
            _dbReader.FinishedReadingAlbums += DbAdapterFinishedReadingAlbums;
            _dbReader.ProgressChanged += DbAdapterProgressChanged;
            _dbReader.StartedReadingAlbums += _dbAdapter_StartedReadingAlbums;

            this.ScanAllText = "SCAN ALL";
            _isTaskbarSupported = TaskbarManager.IsPlatformSupported;

            SetupCommandBindings();

            this.CanShowReloadButton = true;
            this.CanShowScanAllButton = true;

            this.SortOrder = Settings.Default.SortOrder;

            if (_model.SelectedAlbum != null && _model.SelectedAlbum.AlbumDetails.NeedsRefreshing)
            {
                this.Albums.Where(x=> x == _model.SelectedAlbum.AlbumDetails).First().RefreshAlbum();
                UpdateLinkTotals();
                _model.SelectedAlbum.AlbumDetails.NeedsRefreshing = false;
            }

            Messenger.Default.Register<string>(this, HandleMessages);

            _model.AlbumsFromDatabase.CollectionChanged += AlbumsFromDatabase_CollectionChanged;
        }

        void AlbumsFromDatabase_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateLinkTotals();
        }

        private void HandleMessages(string message)
        {
            if (message == "SORT")
            {
                SortData(Settings.Default.SortOrder);
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

        public void SortData(SortOrder sortOrder)
        {
            Settings.Default.SortOrder = sortOrder;
            PerformSort(sortOrder);
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

        private void downloader_FinishedDownloadingAlbums()
        {
            this.CanShowProgressBar = false;
            this.CanShowReloadButton = true;
            ResetLoadingProgress();
        }

        private void downloader_ProgressChanged(int arg1, int arg2)
        {
            //arg1 = current album ;arg2 = total albums
            ReportProgress(arg1, arg2);
            UpdateLinkTotals();
        }

        private void ResetLoadingProgress()
        {
            this.LoadingProgress = 0;

            if (_isTaskbarSupported)
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }

        private void ReportProgress(int current, int total)
        {
            this.LoadingProgress = current * 100 / total;

            if (_isTaskbarSupported)
                TaskbarManager.Instance.SetProgressValue(current, total);
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged("LinkedTotal");
            this.RaisePropertyChanged("UnlinkedTotal");
            this.RaisePropertyChanged("AlbumOrArtistMismatchTotal");
        }
    }
}
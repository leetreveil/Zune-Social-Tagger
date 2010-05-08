using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Taskbar;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class WebAlbumListViewModel : ViewModelBaseExtended
    {
        private readonly IZuneDatabaseReader _dbReader;
        private bool _canShowScanAllButton;
        private int _loadingProgress;
        private bool _canShowProgressBar;
        private bool _canShowReloadButton;
        private readonly bool _isTaskbarSupported;
        private SortOrder _sortOrder;
        private ZuneObservableCollection<AlbumDetailsViewModel> _albums;

        public WebAlbumListViewModel(IZuneDatabaseReader dbReader)
        {
            _dbReader = dbReader;
            _dbReader.FinishedReadingAlbums += DbAdapterFinishedReadingAlbums;
            _dbReader.ProgressChanged += DbAdapterProgressChanged;
            _dbReader.StartedReadingAlbums += _dbAdapter_StartedReadingAlbums;

            _isTaskbarSupported = TaskbarManager.IsPlatformSupported;

            SetupCommandBindings();

            this.CanShowReloadButton = true;
            this.CanShowScanAllButton = true;

            this.SortOrder = Settings.Default.SortOrder;

            Messenger.Default.Register<string>(this, HandleMessages);
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
            this.SwitchToClassicModeCommand = new RelayCommand(SwitchToClassicMode);
            this.SortCommand = new RelayCommand(Sort);
        }

        #region View Binding Properties

        public bool IsCurrentAlbumLinkable
        {
            get
            {
                return true;
            }
        }

        public AlbumDetailsViewModel SelectedAlbum { get; set; }

        public ZuneObservableCollection<AlbumDetailsViewModel> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                _albums.NeedsUpdating += UpdateLinkTotals;
                _albums.CollectionChanged += (sender, args) => {
                    if (args.Action == NotifyCollectionChangedAction.Add || 
                        args.Action ==  NotifyCollectionChangedAction.Remove)
                    {
                        UpdateLinkTotals();
                    }
                };
            }
        }

        public bool CanShowProgressBar
        {
            get { return _canShowProgressBar; }
            set
            {
                _canShowProgressBar = value;
                RaisePropertyChanged(() => this.CanShowProgressBar);
            }
        }

        public bool CanShowReloadButton
        {
            get { return _canShowReloadButton; }
            set
            {
                _canShowReloadButton = value;
                RaisePropertyChanged(() => this.CanShowReloadButton);
            }
        }

        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set
            {
                _sortOrder = value; 
                RaisePropertyChanged(() => this.SortOrder);
            }
        }

        public bool CanShowScanAllButton
        {
            get { return _canShowScanAllButton; }
            set
            {
                _canShowScanAllButton = value;
                RaisePropertyChanged(() => this.CanShowScanAllButton);
            }
        }

        public int LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                RaisePropertyChanged(() => this.LoadingProgress);
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

        public RelayCommand LoadDatabaseCommand { get; private set; }
        public RelayCommand LoadFromZuneWebsiteCommand { get; private set; }
        public RelayCommand CancelDownloadingCommand { get; private set; }
        public RelayCommand SwitchToClassicModeCommand { get; private set; }
        public RelayCommand SortCommand { get; private set; }

        #endregion

        public void LoadFromZuneWebsite()
        {
            ZuneMessageBox.Show(new ErrorMessage(ErrorMode.Warning,"This process could take a very long time, are you sure?"),() => {
                this.CanShowProgressBar = true;
                this.CanShowScanAllButton = false;
                this.CanShowReloadButton = false;

                //skip the unlinked albums that cant be downloaded
                var totalToDownload = this.Albums.Where(x => x.LinkStatus != LinkStatus.Unlinked).ToList();

                int counter = 0;
                foreach (var album in totalToDownload)
                {
                    album.LinkStatus = LinkStatus.Unknown; // reset the linkstatus so we can scan all multiple times
                    album.WebAlbumMetaData = null;

                    album.AlbumDetailsDownloaded += () =>
                    {
                        ReportProgress(counter++, totalToDownload.Count);

                        var toBeDownloaded = this.Albums.Where(x => x.LinkStatus == LinkStatus.Unknown);

                        if (toBeDownloaded.Count() == 0) ResetLoadingProgress();
                    };

                    album.GetAlbumDetailsFromWebsite();
                } 
            } );
        }

        public void RefreshDatabase()
        {
            string msg = "Are you sure? All downloaded album details will be reset and the database will be reloaded.";

            ZuneMessageBox.Show(new ErrorMessage(ErrorMode.Warning, msg), () =>
                 {
                     this.SortOrder =SortOrder. NotSorted;
                     Messenger.Default.Send<string,ApplicationViewModel>("SWITCHTODB");
                 });
        }

        public void SortData(SortOrder sortOrder)
        {
            Settings.Default.SortOrder = sortOrder;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                switch (sortOrder)
                {
                    case SortOrder.DateAdded:
                        this.Albums.SortDesc(x => x.ZuneAlbumMetaData.DateAdded);
                        break;
                    case SortOrder.Album:
                        this.Albums.Sort(x => x.ZuneAlbumMetaData.Title);
                        break;
                    case SortOrder.Artist:
                        this.Albums.Sort(x => x.ZuneAlbumMetaData.Artist);
                        break;
                    case SortOrder.LinkStatus:
                        this.Albums.Sort(x => x.LinkStatus);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        public void SwitchToClassicMode()
        {
            Messenger.Default.Send<Type,ApplicationViewModel>(typeof(SelectAudioFilesViewModel));
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
            this.CanShowScanAllButton = true;
            ResetLoadingProgress();
            this.SortOrder = Settings.Default.SortOrder;
            SortData(Settings.Default.SortOrder);
        }

        private void ResetLoadingProgress()
        {
            this.CanShowScanAllButton = true;
            this.CanShowProgressBar = false;
            this.CanShowReloadButton = true;
            this.LoadingProgress = 0;

            if (_isTaskbarSupported)
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }

        private void ReportProgress(int current, int total)
        {
            this.LoadingProgress = current * 100 / total;

            try
            {
                if (_isTaskbarSupported)
                    TaskbarManager.Instance.SetProgressValue(current, total);
            }
            catch 
            {
                //needs ignoring because it is possible that we try to report progress before a window has been
                //created, this will throw an InvalidOperationException
            }
        }

        private void Sort()
        {
            SortData(this.SortOrder);
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged(() => this.LinkedTotal);
            this.RaisePropertyChanged(() => this.UnlinkedTotal);
            this.RaisePropertyChanged(() => this.AlbumOrArtistMismatchTotal);
        }
    }
}
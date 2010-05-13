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
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    public class WebAlbumListViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private bool _canShowScanAllButton;
        private int _loadingProgress;
        private bool _canShowProgressBar;
        private readonly bool _isTaskbarSupported;
        private SortOrder _sortOrder;
        private bool _canShowSort;
        private ZuneObservableCollection<AlbumDetailsViewModel> _albums;

        public WebAlbumListViewModel(IZuneDatabaseReader dbReader)
        {
            _dbReader = dbReader;
            _dbReader.ProgressChanged += DbAdapterProgressChanged;
            _dbReader.StartedReadingAlbums += _dbAdapter_StartedReadingAlbums;

            _isTaskbarSupported = TaskbarManager.IsPlatformSupported;

            SetupCommandBindings();

            this.CanShowSort = false;
            this.CanShowScanAllButton = true;

            this.SortOrder = Settings.Default.SortOrder;

            Messenger.Default.Register<string>(this, HandleMessages);
        }

        private void HandleMessages(string message)
        {
            if (message == "REFRESHCURRENTALBUM")
            {
                this.SelectedAlbum.RefreshAlbum();
            }
            if (message == "FINISHEDLOADING")
            {
                this.CanShowScanAllButton = true;
                ResetLoadingProgress();
                this.SortOrder = Settings.Default.SortOrder;
                this.CanShowSort = true;
            }
        }

        private void SetupCommandBindings()
        {
            this.LoadFromZuneWebsiteCommand = new RelayCommand(LoadFromZuneWebsite);
            this.SwitchToClassicModeCommand = new RelayCommand(SwitchToClassicMode);
            this.SortCommand = new RelayCommand(Sort);
        }

        #region View Binding Properties

        public bool CanShowSort
        {
            get { return _canShowSort; }
            set
            {
                _canShowSort = value;
                RaisePropertyChanged(() => this.CanShowSort);
            }
        }

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

                //skip the unlinked albums that cant be downloaded
                var totalToDownload = this.Albums.Where(x => x.LinkStatus != LinkStatus.Unlinked).ToList();

                int counter = 0;
                foreach (var album in totalToDownload)
                {
                    album.LinkStatus = LinkStatus.Unknown; // reset the linkstatus so we can scan all multiple times
                    album.WebAlbumMetaData = null;

                    int counter1 = counter;
                    album.AlbumDetailsDownloaded += () =>
                    {
                        ReportProgress(counter1++, totalToDownload.Count);

                        var toBeDownloaded = this.Albums.Where(x => x.LinkStatus == LinkStatus.Unknown);

                        if (toBeDownloaded.Count() == 0) ResetLoadingProgress();
                    };

                    album.GetAlbumDetailsFromWebsite();
                } 
            } );
        }

        public void SwitchToClassicMode()
        {
            Messenger.Default.Send<Type,ApplicationViewModel>(typeof(SelectAudioFilesViewModel));
        }

        private void _dbAdapter_StartedReadingAlbums()
        {
            this.CanShowProgressBar = true;
            this.CanShowScanAllButton = false;
        }

        private void DbAdapterProgressChanged(int arg1, int arg2)
        {
            ReportProgress(arg1, arg2);
        }

        private void ResetLoadingProgress()
        {
            this.CanShowScanAllButton = true;
            this.CanShowProgressBar = false;
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
            Messenger.Default.Send<string,ApplicationViewModel>("SORT");
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged(() => this.LinkedTotal);
            this.RaisePropertyChanged(() => this.UnlinkedTotal);
            this.RaisePropertyChanged(() => this.AlbumOrArtistMismatchTotal);
        }
    }
}
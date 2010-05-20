using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.WindowsAPICodePack.Taskbar;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using GalaSoft.MvvmLight.Threading;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    public class WebAlbumListViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly IViewModelLocator _locator;
        private bool _canShowScanAllButton;
        private int _loadingProgress;
        private bool _canShowProgressBar;
        private readonly bool _isTaskbarSupported;
        private SortOrder _sortOrder;
        private bool _canShowSort;

        public WebAlbumListViewModel(IZuneDatabaseReader dbReader,
                                     ObservableCollection<AlbumDetailsViewModel> albums,
                                     IViewModelLocator locator)
        {
            this.Albums = albums;
            this.AlbumsViewSource = new CollectionViewSource();
            this.AlbumsViewSource.Filter += AlbumsViewSource_Filter;
            this.AlbumsViewSource.Source = this.Albums;

            //manually hook up property changes
            this.Albums.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (AlbumDetailsViewModel item in args.NewItems)
                        item.PropertyChanged += delegate { UpdateLinkTotals(); };

                    UpdateLinkTotals();
                }
                if (args.Action == NotifyCollectionChangedAction.Remove ||
                    args.Action == NotifyCollectionChangedAction.Reset)
                {
                    UpdateLinkTotals();
                }
            };

            _dbReader = dbReader;
            _locator = locator;
            _dbReader.ProgressChanged += DbAdapterProgressChanged;
            _dbReader.StartedReadingAlbums += delegate { this.CanShowScanAllButton = false; };
            _dbReader.FinishedReadingAlbums += delegate {
                DispatcherHelper.CheckBeginInvokeOnUI(Sort);
            };

            _isTaskbarSupported = TaskbarManager.IsPlatformSupported;

            SetupCommandBindings();

            this.CanShowSort = false;
            this.CanShowScanAllButton = true;

            this.SortOrder = Settings.Default.SortOrder;

            Messenger.Default.Register<string>(this, HandleMessages);
        }

        void AlbumsViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item != null && this.FilterText != null)
            {
                var details = (AlbumDetailsViewModel) e.Item;

                e.Accepted = (details.ZuneAlbumMetaData.Artist
                    .ToLower()
                    .StartsWith(this.FilterText.ToLower()) 
                    ||
                    details.ZuneAlbumMetaData.Title.ToLower()
                    .StartsWith(this.FilterText.ToLower()));

            }
        }

        private void HandleMessages(string message)
        {
            if (message == "REFRESHCURRENTALBUM")
            {
                this.SelectedAlbum.RefreshAlbum();
            }
        }

        public void DataHasLoaded()
        {
            this.CanShowScanAllButton = true;
            ResetLoadingProgress();
            this.SortOrder = Settings.Default.SortOrder;
            this.CanShowSort = true;
        }

        private void SetupCommandBindings()
        {
            this.LoadFromZuneWebsiteCommand = new RelayCommand(LoadFromZuneWebsite);
            this.SwitchToClassicModeCommand = new RelayCommand(SwitchToClassicMode);
            this.SortCommand = new RelayCommand(Sort);
            this.SearchCommand = new RelayCommand<string>(Search);
        }

        private void Search(string obj)
        {
            this.AlbumsViewSource.View.Refresh();
            this.AlbumsViewSource.View.Refresh();
            //_albums.Filter(x=> x.ZuneAlbumMetaData.Artist.ToLower().StartsWith(obj.ToLower()));
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

        public string FilterText { get; set; }

        public CollectionViewSource AlbumsViewSource { get; set; }

        public AlbumDetailsViewModel SelectedAlbum { get; set; }

        public ObservableCollection<AlbumDetailsViewModel> Albums { get; set; }

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
        public RelayCommand<string > SearchCommand { get; set; }

        #endregion

        public void LoadFromZuneWebsite()
        {
            ZuneMessageBox.Show(new ErrorMessage(ErrorMode.Warning,"This process could take a very long time, are you sure?"),() => {
                this.CanShowProgressBar = true;
                this.CanShowScanAllButton = false;

                //skip the unlinked albums that cant be downloaded
                var albumsToDownload = this.Albums.Where(x => x.LinkStatus != LinkStatus.Unlinked).ToList();

                foreach (var album in albumsToDownload)
                {
                    album.LinkStatus = LinkStatus.Unknown; // reset the linkstatus so we can scan all multiple times
                    album.WebAlbumMetaData = null;

                    album.AlbumDetailsDownloaded += () =>
                    {
                        var albumsToBeDownloaded = this.Albums.Where(x => x.LinkStatus == LinkStatus.Unknown);

                        int totalDownloaded = albumsToDownload.Count - albumsToBeDownloaded.Count();

                        ReportProgress(totalDownloaded, albumsToDownload.Count);
                        if (albumsToBeDownloaded.Count() == 0) ResetLoadingProgress();
                    };

                    album.GetAlbumDetailsFromWebsite();
                } 
            } );
        }

        public void SwitchToClassicMode()
        {
            _locator.SwitchToViewModel<SelectAudioFilesViewModel>();
        }

        private void DbAdapterProgressChanged(int arg1, int arg2)
        {
            if (!this.CanShowProgressBar)
            {
                this.CanShowProgressBar = true;
            }
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
            catch (InvalidOperationException ex)
            {
                //needs ignoring because it is possible that we try to report progress before a window has been
                //created, this will throw an InvalidOperationException
            }
        }

        public void Sort()
        {
            this.AlbumsViewSource.SortDescriptions.Clear();
            
            switch (this.SortOrder)
            {
                case SortOrder.DateAdded:
                    this.AlbumsViewSource.SortDescriptions.Add(new SortDescription("DateAdded", ListSortDirection.Descending));
                    break;;
                case SortOrder.Album:
                    this.AlbumsViewSource.SortDescriptions.Add(new SortDescription("AlbumTitle", ListSortDirection.Ascending));
                    break;
                case SortOrder.Artist:
                    this.AlbumsViewSource.SortDescriptions.Add(new SortDescription("AlbumArtist", ListSortDirection.Ascending));
                    break;
                case SortOrder.LinkStatus:
                    this.AlbumsViewSource.SortDescriptions.Add(new SortDescription("LinkStatus", ListSortDirection.Ascending));
                    break;
                case SortOrder.NotSorted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.AlbumsViewSource.View.Refresh();

            Settings.Default.SortOrder = this.SortOrder;
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged(() => this.LinkedTotal);
            this.RaisePropertyChanged(() => this.UnlinkedTotal);
            this.RaisePropertyChanged(() => this.AlbumOrArtistMismatchTotal);
        }
    }
}
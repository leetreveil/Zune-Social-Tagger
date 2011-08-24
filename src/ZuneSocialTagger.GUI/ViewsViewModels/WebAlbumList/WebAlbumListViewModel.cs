using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAPICodePack.Taskbar;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using ZuneSocialTagger.GUI.Shared;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    public class WebAlbumListViewModel : ViewModelBase
    {
        private readonly ViewLocator _locator;
        private int _loadingProgress;
        private readonly bool _isTaskbarSupported;
        private SortOrder _sortOrder;
        private string _filterText;
        private readonly SafeObservableCollection<AlbumDetailsViewModel> _albums;
        private readonly CollectionViewSource _cvs;
        private string _scanButtonText = "SCAN ALL";
        private bool _isScanning = false;

        public WebAlbumListViewModel(SafeObservableCollection<AlbumDetailsViewModel> albums,
                                     ViewLocator locator)
        {
            _albums = albums;
            _albums.CollectionChanged += AlbumsCollectionChanged;
            _cvs = new CollectionViewSource();
            _cvs.Source = albums;
            _cvs.Filter += CvsFilter;

            _locator = locator;
            _isTaskbarSupported = TaskbarManager.IsPlatformSupported;

            this.LoadFromZuneWebsiteCommand = new RelayCommand(LoadFromZuneWebsite);
            this.SwitchToClassicModeCommand = new RelayCommand(SwitchToClassicMode);
            this.SortCommand = new RelayCommand(Sort);
            this.SearchCommand = new RelayCommand<string>(Search);
            this.SortOrder = Settings.Default.SortOrder;
        }

        #region View Binding Properties

        public ICollectionView AlbumsView { get { return _cvs.View; } }

        private AlbumDetailsViewModel _selectedAlbum;
        public AlbumDetailsViewModel SelectedAlbum
        {
            get { return _selectedAlbum; }
            set
            {
                _selectedAlbum = value;
                RaisePropertyChanged(() => this.SelectedAlbum);
            }
        }

        public string ScanButtonText
        {
            get { return _scanButtonText; }
            set
            {
                _scanButtonText = value;
                RaisePropertyChanged(() => this.ScanButtonText);
            }
        }

        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set
            {
                _sortOrder = value;
                Settings.Default.SortOrder = value;
                RaisePropertyChanged(() => this.SortOrder);
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
            get 
            {
                return _albums.Where(x => x.LinkStatus == LinkStatus.Linked).Count(); 
            }
        }

        public int UnlinkedTotal
        {
            get
            {
                return _albums.Where(x => x.LinkStatus == LinkStatus.Unlinked).Count();
            }
        }

        public RelayCommand LoadDatabaseCommand { get; private set; }
        public RelayCommand LoadFromZuneWebsiteCommand { get; private set; }
        public RelayCommand CancelDownloadingCommand { get; private set; }
        public RelayCommand SwitchToClassicModeCommand { get; private set; }
        public RelayCommand SortCommand { get; private set; }
        public RelayCommand<string> SearchCommand { get; set; }

        #endregion

        private void Search(string obj)
        {
            this._filterText = obj;
            _cvs.View.Refresh();
        }

        public void LoadFromZuneWebsite()
        {
            if (_isScanning)
            {
                _isScanning = false;
                AlbumDetailsDownloader.AbortAllCurrentRequests();
                ResetLoadingProgress();
                ScanButtonText = "SCAN ALL";
                return;
            }

            var warningMsg = new ErrorMessage(ErrorMode.Warning, "This process could take a very long time, are you sure?");
            var result = ZuneMessageBox.Show(warningMsg, System.Windows.MessageBoxButton.OKCancel);

            if (result == System.Windows.MessageBoxResult.OK)
            {
                this._isScanning = true;
                this.ScanButtonText = "STOP";

                AlbumDetailsDownloader.Aborted = false;

                var toScan = (from object album in _cvs.View select album as AlbumDetailsViewModel)
                    .ToList().Where(x => x.AlbumMediaId != Guid.Empty);

                var splitted = toScan.Split(10);
                //we are scanning the albums in chunks because it is easier for the abortion mechanism
                ScanChunk(splitted, 0, toScan.Count());
            }
        }

        private int scanCount = 0;

        private void ScanChunk(List<List<AlbumDetailsViewModel>> toScan, int position, int albumsTotal)
        {
            int counter = 0;
            var currentChunk = toScan[position];

            foreach (var album in currentChunk)
            {
                AlbumDetailsViewModel closedAlbum = album;

                var url = String.Concat(Urls.Album, album.AlbumMediaId);
                AlbumDetailsDownloader.DownloadAsync(url, (exception, dledAlbum) =>
                {
                    //if the request was cancelled by the user then we dont want to do anything
                    if (exception != null && exception.Status != System.Net.WebExceptionStatus.RequestCanceled)
                    {
                        closedAlbum.LinkStatus = LinkStatus.Unlinked;
                    }

                    if (dledAlbum != null)
                    {
                        closedAlbum.LinkStatus = LinkStatus.Linked;
                        closedAlbum.Right = new AlbumThumbDetails
                        {
                            Artist = dledAlbum.Artist,
                            ArtworkUrl = dledAlbum.ArtworkUrl,
                            Title = dledAlbum.Title
                        };
                    }

                    scanCount++;
                    ReportProgress(scanCount, albumsTotal);

                    counter++;
                    if (counter == currentChunk.Count())
                    {
                        if (AlbumDetailsDownloader.Aborted == false)
                        {
                            position++;
                            Trace.WriteLine(position);
                            if (position < toScan.Count)
                            {
                                ScanChunk(toScan, position, albumsTotal);
                            }
                            else
                            {
                                Debugger.Break();
                            }
                        }
                    }
                });
            }
        }

        public void SwitchToClassicMode()
        {
            _locator.SwitchToView<SelectAudioFilesView, SelectAudioFilesViewModel>();
        }

        private void ReportProgress(int current, int total)
        {
            double percent = current * 100 / total;

            //make sure we dont report progress on every single count
            //just report when the percentage changes
            if (this.LoadingProgress != percent)
            {
                this.LoadingProgress = (int) percent;

                try
                {
                    if (_isTaskbarSupported)
                        TaskbarManager.Instance.SetProgressValue(current, total);
                }
                catch (InvalidOperationException)
                {
                    //needs ignoring because it is possible that we try to report progress before a window has been
                    //created, this will throw an InvalidOperationException
                }
            }

            if (current == total)
            {
                ResetLoadingProgress();
            }    
        }

        private void ResetLoadingProgress()
        {
            scanCount = 0;
            this._isScanning = false;
            this.ScanButtonText = "SCAN ALL";
            this.LoadingProgress = 0;
            DispatcherHelper.CheckBeginInvokeOnUI(delegate { Mouse.OverrideCursor = null; });
            
            try
            {
                if (_isTaskbarSupported)
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
            }
        }

        public void Sort()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                using (_cvs.DeferRefresh())
                {
                    _cvs.SortDescriptions.Clear();

                    switch (this.SortOrder)
                    {
                        case SortOrder.DateAdded:
                            _cvs.SortDescriptions.Add(new SortDescription("DateAdded", ListSortDirection.Descending));
                            break;
                        case SortOrder.Album:
                            _cvs.SortDescriptions.Add(new SortDescription("Left.Title", ListSortDirection.Ascending));
                            break;
                        case SortOrder.Artist:
                            _cvs.SortDescriptions.Add(new SortDescription("Left.Artist", ListSortDirection.Ascending));
                            break;
                        case SortOrder.LinkStatus:
                            _cvs.SortDescriptions.Add(new SortDescription("LinkStatus", ListSortDirection.Ascending));
                            break;
                        case SortOrder.NotSorted:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged(() => this.LinkedTotal);
            this.RaisePropertyChanged(() => this.UnlinkedTotal);
        }

        void CvsFilter(object sender, FilterEventArgs e)
        {
            if (e.Item != null && _filterText != null)
            {
                var details = (AlbumDetailsViewModel)e.Item;

                //match the artist or the album title
                e.Accepted = (details.Left.Artist
                    .ToLower()
                    .Contains(_filterText.ToLower())
                    ||
                    details.Left.Title.ToLower()
                    .Contains(_filterText.ToLower()));

            }
        }

        void AlbumsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AlbumDetailsViewModel item in e.NewItems)
                    item.PropertyChanged += delegate { UpdateLinkTotals(); };
            }

            UpdateLinkTotals();
        }
    }
}
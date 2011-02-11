using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAPICodePack.Taskbar;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using GalaSoft.MvvmLight.Threading;
using System.Threading;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    public class WebAlbumListViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly IViewLocator _locator;
        private bool _canShowScanAllButton;
        private int _loadingProgress;
        private readonly bool _isTaskbarSupported;
        private SortOrder _sortOrder;
        private bool _canShowSort;
        private string _filterText;

        public WebAlbumListViewModel(IZuneDatabaseReader dbReader,
                                     SafeObservableCollection<AlbumDetailsViewModel> albums,
                                     IViewLocator locator)
        {
            this.Albums  = albums;

            this.AlbumsViewSource = new CollectionViewSource();
            this.AlbumsViewSource.Source = this.Albums;
            this.AlbumsViewSource.Filter += AlbumsViewSource_Filter;

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
            _dbReader.ProgressChanged += ReportProgress;
            _dbReader.StartedReadingAlbums += () => this.CanShowScanAllButton = false;

            _isTaskbarSupported = TaskbarManager.IsPlatformSupported;

            SetupCommandBindings();

            this.CanShowSort = true;
            this.CanShowScanAllButton = true;
            this.SortOrder = Settings.Default.SortOrder;
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

        public CollectionViewSource AlbumsViewSource { get; set; }

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

        public SafeObservableCollection<AlbumDetailsViewModel> Albums { get; set; }

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
                return this.Albums.Where(x => x.LinkStatus == LinkStatus.Unlinked).Count();
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
        public RelayCommand<string> SearchCommand { get; set; }

        #endregion

        void AlbumsViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item != null && _filterText != null)
            {
                var details = (AlbumDetailsViewModel)e.Item;

                //match the artist or the album title
                e.Accepted = (details.ZuneAlbumMetaData.Artist
                    .ToLower()
                    .StartsWith(_filterText.ToLower())
                    ||
                    details.ZuneAlbumMetaData.Title.ToLower()
                    .StartsWith(_filterText.ToLower()));

            }
        }

        public void DataHasLoaded()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>{
                this.CanShowScanAllButton = true;
                ResetLoadingProgress();
                this.CanShowSort = true;
                Sort();
            });
        }

        public void SuspendSorting() {
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                this.AlbumsViewSource.SortDescriptions.Clear();
            });
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
            this._filterText = obj;
            this.AlbumsViewSource.View.Refresh();
        }

        public void LoadFromZuneWebsite()
        {
            var warningMsg = new ErrorMessage(ErrorMode.Warning,"This process could take a very long time, are you sure?");
            ZuneMessageBox.Show(warningMsg, () => {
                this.CanShowScanAllButton = false;

                //make sure the source collection matches the one bound to the view so albums are scanned in the correct order
                SortSourceCollection();

                //skip the unlinked albums that cant be downloaded
                //var albumsToDownload = .ToList();

                //Trace.WriteLine("total length: " + albumsToDownload.Count);

                int counter = 0;
                foreach (var album in this.Albums)
                {
                    album.LinkStatus = LinkStatus.Unknown; // reset the linkstatus so we can scan all multiple times
                    album.WebAlbumMetaData = null;

                    album.DownloadCompleted += () => 
                    {
                        counter++;
                        ReportProgress(counter, this.Albums.Count);
                        if (counter == this.Albums.Count) ResetLoadingProgress();
                    };

                    album.GetAlbumDetailsFromWebsite();
                } 
            });
        }

        public void SwitchToClassicMode()
        {
            _locator.SwitchToView<SelectAudioFilesView,SelectAudioFilesViewModel>();
        }

        private void ResetLoadingProgress()
        {
            this.CanShowScanAllButton = true;

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
                catch (InvalidOperationException ex)
                {
                    //needs ignoring because it is possible that we try to report progress before a window has been
                    //created, this will throw an InvalidOperationException
                }
            }
        }

        public void Sort()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                using (this.AlbumsViewSource.DeferRefresh())
                {
                    this.AlbumsViewSource.SortDescriptions.Clear();

                    switch (this.SortOrder)
                    {
                        case SortOrder.DateAdded:
                            this.AlbumsViewSource.SortDescriptions.Add(new SortDescription("DateAdded", ListSortDirection.Descending));
                            break;
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
                }
            });
        }

        private void SortSourceCollection() {
            IEnumerable<AlbumDetailsViewModel> sortedCollection = null;
            switch (this.SortOrder)
            {
                case SortOrder.DateAdded:
                    sortedCollection = this.Albums.OrderByDescending(x => x.DateAdded).ToList();
                    break;
                case SortOrder.Album:
                    sortedCollection = this.Albums.OrderBy(x => x.AlbumTitle).ToList();
                    break;
                case SortOrder.Artist:
                    sortedCollection = this.Albums.OrderBy(x => x.AlbumArtist).ToList();
                    break;
                case SortOrder.LinkStatus:
                    sortedCollection = this.Albums.OrderBy(x => x.LinkStatus).ToList();
                    break;
                case SortOrder.NotSorted:
                    break;
                default:
                    break;
            }

            if (sortedCollection != null)
            {
                ClearAndAddToSource(sortedCollection);
            }
        }

        private void ClearAndAddToSource(IEnumerable<AlbumDetailsViewModel> items) {
            this.Albums.Clear();
            foreach (var item in items)
            {
                this.Albums.Add(item);
            }
        }

        private void UpdateLinkTotals()
        {
            this.RaisePropertyChanged(() => this.LinkedTotal);
            this.RaisePropertyChanged(() => this.UnlinkedTotal);
            this.RaisePropertyChanged(() => this.AlbumOrArtistMismatchTotal);
        }
    }
}
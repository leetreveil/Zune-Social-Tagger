using System;
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
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using GalaSoft.MvvmLight.Threading;

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
        private readonly SafeObservableCollection<AlbumDetailsViewModel> _albums;
        private readonly CollectionViewSource _cvs;

        public WebAlbumListViewModel(IZuneDatabaseReader dbReader,
                                     SafeObservableCollection<AlbumDetailsViewModel> albums,
                                     IViewLocator locator)
        {
            _albums = albums;
            _albums.CollectionChanged += AlbumsCollectionChanged;
            _cvs = new CollectionViewSource();
            _cvs.Source = albums;
            _cvs.Filter += CvsFilter;

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

        public ICollectionView AlbumsView { get { return _cvs.View; } }

        public bool CanShowSort
        {
            get { return _canShowSort; }
            set
            {
                _canShowSort = value;
                RaisePropertyChanged(() => this.CanShowSort);
            }
        }

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
            get { return _albums.Where(x => x.LinkStatus == LinkStatus.Linked).Count(); }
        }

        public int UnlinkedTotal
        {
            get
            {
                return _albums.Where(x => x.LinkStatus == LinkStatus.Unlinked).Count();
            }
        }

        public int AlbumOrArtistMismatchTotal
        {
            get { return _albums.Where(x => x.LinkStatus == LinkStatus.AlbumOrArtistMismatch).Count(); }
        }

        public RelayCommand LoadDatabaseCommand { get; private set; }
        public RelayCommand LoadFromZuneWebsiteCommand { get; private set; }
        public RelayCommand CancelDownloadingCommand { get; private set; }
        public RelayCommand SwitchToClassicModeCommand { get; private set; }
        public RelayCommand SortCommand { get; private set; }
        public RelayCommand<string> SearchCommand { get; set; }

        #endregion

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
                _cvs.SortDescriptions.Clear();
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
            _cvs.View.Refresh();
        }

        public void LoadFromZuneWebsite()
        {
            var warningMsg = new ErrorMessage(ErrorMode.Warning,"This process could take a very long time, are you sure?");
            ZuneMessageBox.Show(warningMsg, () => {
                this.CanShowScanAllButton = false;

                int counter = 0;
                foreach (AlbumDetailsViewModel album in _cvs.View)
                {
                    album.LinkStatus = LinkStatus.Unknown; // reset the linkstatus so we can scan all multiple times
                    album.Right = null;
                    
                    AlbumDetailsViewModel closedAlbum = album;
                    //TODO: don't like having to call back into the zune db just to get the albumMediaId
                    var albumMediaId = _dbReader.GetAlbum(album.MediaId).AlbumMediaId;
                    var url = String.Concat(Urls.Album, albumMediaId);
                    AlbumDetailsDownloader.DownloadAsync(url, dledAlbum =>
                    {
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
                        else
                        {
                            closedAlbum.LinkStatus = LinkStatus.Unlinked;
                        }

                        counter++;
                        ReportProgress(counter, _albums.Count);
                        if (counter == _albums.Count) ResetLoadingProgress();
                    });
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
            this.RaisePropertyChanged(() => this.AlbumOrArtistMismatchTotal);
        }

        void CvsFilter(object sender, FilterEventArgs e)
        {
            if (e.Item != null && _filterText != null)
            {
                var details = (AlbumDetailsViewModel)e.Item;

                //match the artist or the album title
                e.Accepted = (details.Left.Artist
                    .ToLower()
                    .StartsWith(_filterText.ToLower())
                    ||
                    details.Left.Title.ToLower()
                    .StartsWith(_filterText.ToLower()));

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
using System;
using Caliburn.PresentationFramework;
using Microsoft.Practices.ServiceLocation;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WebAlbumListViewModel : Screen
    {
        private readonly IServiceLocator _locator;
        private readonly IZuneWizardModel _model;
        private IZuneDbAdapter _dbAdapter;
        private bool _isLoading;
        private int _loadingProgress;
        private BindableCollection<AlbumDetailsViewModel> _albums;
        private AlbumDownloaderWithProgressReporting _downloader;

        public WebAlbumListViewModel(IServiceLocator locator, IZuneWizardModel model, IZuneDbAdapter dbAdapter)
        {
            _locator = locator;
            _model = model;

            _dbAdapter = dbAdapter;
            _dbAdapter.FinishedReadingAlbums += DbAdapterFinishedReadingAlbums;
            _dbAdapter.ProgressChanged += DbAdapterProgressChanged;

            this.Albums = new BindableCollection<AlbumDetailsViewModel>();

            this.SortViewModel = new SortViewModel();
            this.SortViewModel.SortClicked += SortViewModel_SortClicked;

            LoadAlbumsFromCacheOrZuneDatabase();
        }

        #region View Binding Properties

        public BindableCollection<AlbumDetailsViewModel> Albums
        {
            get { return _albums; }
            set
            {
                _albums = value;
                NotifyOfPropertyChange(() => this.Albums);
            }
        }

        public SortViewModel SortViewModel { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => this.IsLoading);
            }
        }

        public int LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                NotifyOfPropertyChange(() => this.LoadingProgress);
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

        #endregion

        public void LoadFromZuneWebsite()
        {
            this.IsLoading = true;

            foreach (var album in this.Albums)
            {
                album.WebAlbumMetaData = null;

                if (album.LinkStatus != LinkStatus.Unlinked)
                    album.LinkStatus = LinkStatus.Unknown;
            }

            _downloader = new AlbumDownloaderWithProgressReporting(this.Albums);

            _downloader.ProgressChanged += downloader_ProgressChanged;
            _downloader.FinishedDownloadingAlbums += downloader_FinishedDownloadingAlbums;
            _downloader.Start();
        }

        public void CancelDownloading()
        {
            ResetLoadingProgress();

            if (_downloader != null)
                _downloader.StopDownloading();
        }

        public void LoadAlbumsFromCacheOrZuneDatabase()
        {
            this.IsLoading = true;
            this.Albums.Clear();
            this.SortViewModel.SortOrder = SortOrder.NotSorted;

            ThreadPool.QueueUserWorkItem(delegate
            {
                foreach (AlbumDetailsViewModel newAlbum in _dbAdapter.ReadAlbums())
                {
                    //add handler to be notified when the LinkStatus enum changes
                    newAlbum.PropertyChanged += album_PropertyChanged;

                    if (newAlbum.ZuneAlbumMetaData.AlbumMediaId == Guid.Empty)
                        newAlbum.LinkStatus = LinkStatus.Unlinked;

                    this.Albums.Add(newAlbum);
                }

                this.IsLoading = false;
            });
           
        }

        //public void LoadDatabase()
        //{
        //    _dbAdapter = new ZuneDbAdapter(_locator.GetInstance<IZuneDatabaseReader>(),_locator);
        //    _dbAdapter.ProgressChanged +=DbAdapterProgressChanged;
        //    _dbAdapter.FinishedReadingAlbums +=DbAdapterFinishedReadingAlbums;

        //    LoadAlbumsFromCacheOrZuneDatabase();
        //}

        public void SwitchToClassicMode()
        {
            _model.CurrentPage = _locator.GetInstance<SelectAudioFilesViewModel>();
        }

        private void PerformSort(SortOrder sortOrder)
        {
            ThreadPool.QueueUserWorkItem(_ =>
                 {
                     //TODO: remove code repetition
                     switch (sortOrder)
                     {
                         case SortOrder.DateAdded:
                             this.Albums =
                                 this.Albums.OrderByDescending(
                                     x => x.ZuneAlbumMetaData.DateAdded).
                                     ToBindableCollection();
                             break;
                         case SortOrder.Album:
                             this.Albums =
                                 this.Albums.OrderBy(x => x.ZuneAlbumMetaData.AlbumTitle).
                                     ToBindableCollection();
                             break;
                         case SortOrder.Artist:
                             this.Albums =
                                 this.Albums.OrderBy(x => x.ZuneAlbumMetaData.AlbumArtist).
                                     ToBindableCollection();
                             break;
                         case SortOrder.LinkStatus:
                             this.Albums =
                                 this.Albums.OrderByDescending(x => x.LinkStatus).
                                     ToBindableCollection();
                             break;
                         default:
                             throw new ArgumentOutOfRangeException();
                     }
                 });
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
            this.IsLoading = false;
        }

        private void UpdateLinkTotals()
        {
            this.NotifyOfPropertyChange(() => this.LinkedTotal);
            this.NotifyOfPropertyChange(() => this.UnlinkedTotal);
            this.NotifyOfPropertyChange(() => this.AlbumOrArtistMismatchTotal);
        }
    }
}
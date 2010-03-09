using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;
        private readonly IZuneDatabaseReader _dbReader;
        private bool _isLoading;
        private int _loadingProgress;
        private BindableCollection<AlbumDetailsViewModel> _albums;

        public SelectAudioFilesViewModel(IUnityContainer container,
                                         IZuneWizardModel model,
                                         IZuneDatabaseReader dbReader)
        {
            _container = container;
            _model = model;
            _dbReader = dbReader;
            _dbReader.Initialize();
            _dbReader.FinishedReadingAlbums += _dbReader_FinishedReadingAlbums;
            _dbReader.ProgressChanged += _dbReader_ProgressChanged;

            this.Albums = new BindableCollection<AlbumDetailsViewModel>();

            this.SortViewModel = new SortViewModel(this.Albums);
            this.SortViewModel.SortCompleted += SortViewModel_SortCompleted;

            LoadAlbumsFromZuneDatabase();
        }

        private void SortViewModel_SortCompleted(BindableCollection<AlbumDetailsViewModel> sortedAlbums)
        {
            this.Albums = sortedAlbums;
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
            get 
            { 
                return this.Albums.Where(x => x.LinkStatus == LinkStatus.Linked).Count(); 
            }
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
            get
            {
                return this.Albums.Where(x => x.LinkStatus == LinkStatus.AlbumOrArtistMismatch).Count();
            }
        }

        #endregion

        public void LoadFromZuneWebsite()
        {
            this.IsLoading = true;

            var downloader = new AlbumDownloaderWithProgressReporting(this.Albums);

            downloader.ProgressChanged += downloader_ProgressChanged;
            downloader.FinishedDownloadingAlbums += downloader_FinishedDownloadingAlbums;
            downloader.Start();
        }

        public void LoadAlbumsFromZuneDatabase()
        {
            this.IsLoading = true;
            this.Albums.Clear();

            ThreadPool.QueueUserWorkItem(delegate
                 {
                     foreach (Album newAlbum in _dbReader.ReadAlbums())
                     {
                         var album = new AlbumDetailsViewModel(_container, _model, _dbReader);

                         album.ZuneAlbumMetaData = newAlbum;

                         if (album.ZuneAlbumMetaData.AlbumMediaId == Guid.Empty)
                             album.LinkStatus = LinkStatus.Unlinked;

                         album.PropertyChanged += album_PropertyChanged;

                         this.Albums.Add(album);

                         UpdateLinkTotals();
                     }

                     this.IsLoading = false;
                 });
        }

        private void album_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LinkStatus")
                UpdateLinkTotals();   
        }

        private void _dbReader_ProgressChanged(int arg1, int arg2)
        {
            UpdateLinkTotals();
            ReportProgress(arg1, arg2);
        }

        private void ReportProgress(int current, int total)
        {
            this.LoadingProgress = current*100/total;
        }

        private void _dbReader_FinishedReadingAlbums()
        {
            ResetLoadingProgress();

            //TODO: do sorting in place so as albums are being loaded they are being sorted too

            //sorting by date added so you see the newest albums youve added first
            //var sortedByDate = _albums.OrderByDescending(x => x.ZuneAlbumMetaData.DateAdded).ToList();

            //_albums.Clear();

            //foreach (var album in sortedByDate)
            //{
            //    _albums.Add(album);
            //}
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
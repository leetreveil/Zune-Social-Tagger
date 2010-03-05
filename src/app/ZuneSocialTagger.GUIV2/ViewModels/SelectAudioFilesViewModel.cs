using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using Album = ZuneSocialTagger.GUIV2.Models.Album;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;
        private readonly IZuneDatabaseReader _dbReader;
        private bool _albumSortOrder;
        private bool _linkedToSortOrder;
        private int _albumOrArtistMismatchTotal;
        private int _unlinkedTotal;
        private int _linkedTotal;
        private bool _isLoading;
        private int _loadingProgress;

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

            LoadAlbumsFromZuneDatabase();
        }

        #region View Binding Properties

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => this.IsLoading);
            }
        }

        public BindableCollection<AlbumDetailsViewModel> Albums { get; set; }

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
            get { return _linkedTotal; }
            set
            {
                _linkedTotal = value;
                NotifyOfPropertyChange(() => this.LinkedTotal);
            }
        }

        public int UnlinkedTotal
        {
            get { return _unlinkedTotal; }
            set
            {
                _unlinkedTotal = value;
                NotifyOfPropertyChange(() => this.UnlinkedTotal);
            }
        }

        public int AlbumOrArtistMismatchTotal
        {
            get { return _albumOrArtistMismatchTotal; }
            set
            {
                _albumOrArtistMismatchTotal = value;
                NotifyOfPropertyChange(() => this.AlbumOrArtistMismatchTotal);
            }
        }

        #endregion

        public void LoadFromZuneWebsite()
        {
            this.IsLoading = true;

            foreach (var albumVm in Albums)
            {
                //reset the web album details each time because 
                //we want the details to reload each time the user hits reload
                albumVm.Album.WebAlbumMetaData = null;

                //set all albums that dont have a mediaid to unlinked
                //albumVm.Album.LinkStatus = String.IsNullOrEmpty(albumVm.Album.ZuneAlbumMetaData.AlbumMediaId) 
                //    ? LinkStatus.Unlinked 
                //    : LinkStatus.Unknown;
            }

            var downloader = new AlbumDownloaderWithProgressReporting(this.Albums.Select(x => x.Album));

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
                     foreach (Album newAlbum in _dbReader.ReadAlbums().Select(dbAlbumDetails => Album.GetNewAlbum(dbAlbumDetails)))
                         this.Albums.Add(new AlbumDetailsViewModel(newAlbum,_container,_model,_dbReader));

                     this.IsLoading = false;
                 });


            //TODO: change unkown to the default link status in the database
        }

        public void Sort(string header)
        {
            var sortedAlbums = new List<Album>();

            if (header == "Album")
            {
                sortedAlbums = SortBy(x => x.ZuneAlbumMetaData.AlbumTitle, _albumSortOrder);
                _albumSortOrder = !_albumSortOrder;
            }

            if (header == "Linked To")
            {
                sortedAlbums = SortBy(x => x.LinkStatus, _linkedToSortOrder);
                _linkedToSortOrder = !_linkedToSortOrder;
            }

            //TODO: dont like how sorting is working, i.e. having two sort variables

            this.Albums.Clear();

            foreach (var sortedAlbum in sortedAlbums)
                this.Albums.Add(new AlbumDetailsViewModel(sortedAlbum,_container,_model,_dbReader));
        }

        private List<Album> SortBy<T>(Func<Album, T> selector, bool sortOrder)
        {
            List<Album> sortedAlbums = sortOrder
                                           ? this.Albums.Select(x => x.Album).OrderBy(selector).ToList()
                                           : this.Albums.Select(x => x.Album).OrderByDescending(selector).ToList();

            return sortedAlbums;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="arg1">Current Progress</param>
        /// <param name="arg2">Total</param>
        private void _dbReader_ProgressChanged(int arg1, int arg2)
        {
            this.UnlinkedTotal =
                this.Albums.Select(x => x.Album).Where(x => x.LinkStatus == LinkStatus.Unlinked).Count();

            ReportProgress(arg1, arg2);
        }

        private void ReportProgress(int current, int total)
        {
            this.LoadingProgress = current*100/total;
        }

        private void _dbReader_FinishedReadingAlbums()
        {
            this.LoadingProgress = 0;
            this.IsLoading = false;

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
            this.LoadingProgress = 0;
            this.IsLoading = false;
        }

        private void downloader_ProgressChanged(int arg1, int arg2)
        {
            //arg1 = current album
            //arg2 = total albums
            if (arg1 < arg2)
                ReportProgress(arg1, arg2);

            UpdateLinkTotals();
        }

        private void UpdateLinkTotals()
        {
            this.LinkedTotal = this.Albums.Select(x => x.Album).Where(x => x.LinkStatus == LinkStatus.Linked).Count();
            var unlinkedTotal = this.Albums.Select(x => x.Album).Where(x => x.LinkStatus == LinkStatus.Unlinked).Count();
            var couldNotDownloadTotal =
                this.Albums.Select(x => x.Album).Where(x => x.LinkStatus == LinkStatus.Unavailable).Count();

            this.UnlinkedTotal = unlinkedTotal + couldNotDownloadTotal;
            this.AlbumOrArtistMismatchTotal =
                this.Albums.Select(x => x.Album).Where(x => x.LinkStatus == LinkStatus.AlbumOrArtistMismatch).Count();
        }
    }
}
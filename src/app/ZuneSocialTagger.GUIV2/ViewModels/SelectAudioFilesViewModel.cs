using System;
using System.Collections.Generic;
using System.Windows.Input;
using Caliburn.PresentationFramework;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using ZuneSocialTagger.GUIV2.Views;
using Album = ZuneSocialTagger.GUIV2.Models.Album;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using System.IO;
using ZuneSocialTagger.Core;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;
using System.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;

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
        private readonly BindableCollection<Album> _albums;

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
            _albums = _model.DatabaseAlbums;

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

        public BindableCollection<Album> Albums
        {
            get { return _albums; }
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


            //reset the web album details each time because 
            //we want the details to reload each time the user hits reload
            foreach (var album in Albums)
            {
                album.WebAlbumMetaData = null;
                //set all albums that dont have a mediaid to unlinked
                album.LinkStatus = String.IsNullOrEmpty(album.ZuneAlbumMetaData.AlbumMediaId) 
                    ? LinkStatus.Unlinked 
                    : LinkStatus.Unknown;
            }

            var downloader = new AlbumDownloaderWithProgressReporting(_albums);

            downloader.ProgressChanged += downloader_ProgressChanged;
            downloader.FinishedDownloadingAlbums += downloader_FinishedDownloadingAlbums;
            downloader.Start();
        }

        public void LoadAlbumsFromZuneDatabase()
        {
            this.IsLoading = true;
            _albums.Clear();

            ThreadPool.QueueUserWorkItem(delegate
             {
                 //TODO: fix bug where when the artwork from the database is either corrupted or does not exist should not cause an error
                 foreach ( Core.ZuneDatabase.Album dbAlbumDetails in _dbReader.ReadAlbums())
                 {
                     Album newAlbum = GetNewAlbum(dbAlbumDetails);

                     //TODO: need to check if an image is valid or not otherwise the application will crash

                     _albums.Add(newAlbum);
                 }
             });


            //TODO: change unkown to the default link status in the database
        }

        private static Album GetNewAlbum(Core.ZuneDatabase.Album dbAlbumDetails)
        {
            Album newAlbum = new Album();

            string artworkUrl = dbAlbumDetails.ArtworkUrl;

            if (String.IsNullOrEmpty(artworkUrl))
                artworkUrl = "pack://application:,,,/Assets/blankartwork.png";
            else
            {
                var path = new string(artworkUrl.Skip(7).ToArray());

                if (!File.Exists(path))
                    artworkUrl = "pack://application:,,,/Assets/blankartwork.png";
            }

            newAlbum.ZuneAlbumMetaData =
                new AlbumDetails(dbAlbumDetails.MediaId,
                                 dbAlbumDetails.AlbumTitle,
                                 dbAlbumDetails.AlbumArtist,
                                 artworkUrl, dbAlbumDetails.AlbumMediaId,
                                 dbAlbumDetails.DateAdded,
                                 dbAlbumDetails.ReleaseYear,
                                 dbAlbumDetails.TrackCount);


            //set all albums that dont have a mediaid to unlinked
            if (String.IsNullOrEmpty(newAlbum.ZuneAlbumMetaData.AlbumMediaId))
                newAlbum.LinkStatus = LinkStatus.Unlinked;

            return newAlbum;
        }

        public void RefreshAlbum(Album album)
        {
            var doesAlbumExist = _dbReader.DoesAlbumExist(album.ZuneAlbumMetaData.MediaId);

            if (!doesAlbumExist)
            {
                ShowRefreshDatabaseError();  
            }
            else
            {
                var newAlbum = GetNewAlbum(_dbReader.GetAlbum(album.ZuneAlbumMetaData.MediaId));

                string url = String.Concat("http://catalog.zune.net/v3.0/en-US/music/album/",
                                       newAlbum.ZuneAlbumMetaData.AlbumMediaId);

                var downloader = new AlbumDetailsDownloader(url);

                downloader.DownloadCompleted += obj => AlbumDownloaderWithProgressReporting.DownloadAlbum(obj, newAlbum);

                downloader.Download();

                var indexOf = _albums.IndexOf(album);
                _albums.Remove(album);

                _albums.Insert(indexOf, newAlbum);
            }
        }

        public void LinkAlbum(Album albumToBeLinked)
        {
            var doesAlbumExist = _dbReader.DoesAlbumExist(albumToBeLinked.ZuneAlbumMetaData.MediaId);

            if (!doesAlbumExist)
            {
                ShowRefreshDatabaseError();
            }
            else
            {
                AlbumDetails dbAlbumDetails = albumToBeLinked.ZuneAlbumMetaData;

                _model.SearchBarViewModel.SearchText = dbAlbumDetails.AlbumArtist + " " +
                                                       dbAlbumDetails.AlbumTitle;

                _model.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel
                {
                    Artist = dbAlbumDetails.AlbumArtist,
                    Title = dbAlbumDetails.AlbumTitle,
                    ArtworkUrl = dbAlbumDetails.ArtworkUrl,
                    SongCount = dbAlbumDetails.TrackCount.ToString(),
                    Year = dbAlbumDetails.ReleaseYear.ToString()
                };

                IEnumerable<Track> tracksForAlbum = _dbReader.GetTracksForAlbum(dbAlbumDetails.MediaId);

                _model.Rows.Clear();

                foreach (var track in tracksForAlbum)
                    _model.Rows.Add(new DetailRow(track.FilePath, ZuneTagContainerFactory.GetContainer(track.FilePath)));


                _model.CurrentPage = _container.Resolve<SearchViewModel>();
            }
        }


        public void DelinkAlbum(Album album)
        {
            var doesAlbumExist = _dbReader.DoesAlbumExist(album.ZuneAlbumMetaData.MediaId);

            if (!doesAlbumExist)
            {
                ShowRefreshDatabaseError();
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var tracksForAlbum = _dbReader.GetTracksForAlbum(album.ZuneAlbumMetaData.MediaId);

                foreach (var track in tracksForAlbum)
                {
                    var container = ZuneTagContainerFactory.GetContainer(track.FilePath);

                    container.RemoveZuneAttribute("ZuneCollectionID");
                    container.RemoveZuneAttribute(ZuneIds.Album);
                    container.RemoveZuneAttribute(ZuneIds.Artist);
                    container.RemoveZuneAttribute(ZuneIds.Track);

                    container.WriteToFile(track.FilePath);
                }

                Mouse.OverrideCursor = null;

                //TODO: change this to an information message box because it is not an error
                ZuneMessageBox.Show("Album should now be de-linked. You may need to "+ 
                                        "remove then re-add the album for the changes to take effect.",ErrorMode.Warning);  
            }

        }

        private void ShowRefreshDatabaseError()
        {
            ZuneMessageBox.Show("Could not find album, you may need to refresh the database.",ErrorMode.Error);
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

            this._albums.Clear();

            foreach (var sortedAlbum in sortedAlbums)
                this._albums.Add(sortedAlbum);
        }

        private List<Album> SortBy<T>(Func<Album, T> selector, bool sortOrder)
        {
            List<Album> sortedAlbums = sortOrder
                                           ? this._albums.OrderBy(selector).ToList()
                                           : this._albums.OrderByDescending(selector).ToList();

            return sortedAlbums;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="arg1">Current Progress</param>
        /// <param name="arg2">Total</param>
        private void _dbReader_ProgressChanged(int arg1, int arg2)
        {
            this.UnlinkedTotal = _albums.Where(x => x.LinkStatus == LinkStatus.Unlinked).Count();

            ReportProgress(arg1, arg2);
        }

        private void ReportProgress(int current, int total)
        {
            this.LoadingProgress = current * 100 / total;
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
            this.LinkedTotal = _albums.Where(x => x.LinkStatus == LinkStatus.Linked).Count();
            this.UnlinkedTotal = _albums.Where(x => x.LinkStatus == LinkStatus.Unlinked).Count();
            this.AlbumOrArtistMismatchTotal = _albums.Where(x => x.LinkStatus == LinkStatus.AlbumOrArtistMismatch).Count();
        }


    }
}
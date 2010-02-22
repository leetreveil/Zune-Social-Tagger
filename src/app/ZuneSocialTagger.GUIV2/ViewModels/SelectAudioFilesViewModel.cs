using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using Album = ZuneSocialTagger.GUIV2.Models.Album;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using System.IO;
using ZuneSocialTagger.Core;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;
        private readonly IZuneDatabaseReader _dbReader;
        private bool _albumSortOrder;
        private bool _linkedToSortOrder;
        private VirtualizingCollection<Album> _virtualAlbums;
        private List<Album> _albums;
        private int _albumOrArtistMismatchTotal;
        private int _unlinkedTotal;
        private int _linkedTotal;
        private bool _isLoading;
        private readonly AlbumItemProvider _itemsProvider;

        private bool _canSort;

        public SelectAudioFilesViewModel(IUnityContainer container,
                                         IZuneWizardModel model,
                                         IZuneDatabaseReader dbReader)
        {
            _container = container;
            _model = model;
            _dbReader = dbReader;
            _albums = new List<Album>();

            this.LoadAlbumsFromZuneDatabase();

            _itemsProvider = new AlbumItemProvider(_albums);
            _itemsProvider.AllDownloadsComplete += provider_AllDownloadsComplete;
            _itemsProvider.ItemFinishedDownloading += provider_ItemFinishedDownloading;

            this.VirtualAlbums = new VirtualizingCollection<Album>(_itemsProvider, 10);

            //TODO: remove all zune fonts, default is close enough anyway
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

        public bool CanSort
        {
            get { return _canSort; }
            set
            {
                _canSort = value;
                NotifyOfPropertyChange(() => this.CanSort);
            }
        }

        public VirtualizingCollection<Album> VirtualAlbums
        {
            get { return _virtualAlbums; }
            set
            {
                _virtualAlbums = value;
                NotifyOfPropertyChange(() => this.VirtualAlbums);
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

        public void LoadFromZuneWebsite()
        {
            this.IsLoading = true;
            this.VirtualAlbums.LoadAllPages();
        }

        #endregion

        public void LoadAlbumsFromZuneDatabase()
        {
            _dbReader.Initialize();

            //TODO: fix bug where when the artwork from the database is either corrupted or does not exist should not cause an error
            foreach (ZuneSocialTagger.Core.ZuneDatabase.Album dbAlbumDetails in _dbReader.ReadAlbums())
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

                newAlbum.ZuneAlbumMetaData = new AlbumDetails(dbAlbumDetails.MediaId,dbAlbumDetails.AlbumTitle, dbAlbumDetails.AlbumArtist,
                                                              artworkUrl, dbAlbumDetails.AlbumMediaId,
                                                              dbAlbumDetails.DateAdded, dbAlbumDetails.ReleaseYear,
                                                              dbAlbumDetails.TrackCount);


                newAlbum.IsLinked = !String.IsNullOrEmpty(newAlbum.ZuneAlbumMetaData.AlbumMediaId)
                                        ? LinkStatus.Unknown
                                        : LinkStatus.Unlinked;


                //TODO: need to check if an image is valid or not otherwise the application will crash

                _albums.Add(newAlbum);
            }

            //TODO: change unkown to the default link status in the database

            //sorting by date added so you see the newest albums youve added first
            _albums = _albums.OrderByDescending(x => x.ZuneAlbumMetaData.DateAdded).ToList();
        }

        public void LinkAlbum(Album albumToBeLinked)
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

            foreach (var track in tracksForAlbum)
            {
                _model.Rows.Add(new DetailRow(track.FilePath,ZuneTagContainerFactory.GetContainer(track.FilePath)));
            }


            _model.CurrentPage = _container.Resolve<SearchViewModel>();
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
                sortedAlbums = SortBy(x => x.IsLinked, _linkedToSortOrder);
                _linkedToSortOrder = !_linkedToSortOrder;
            }

            //TODO: dont like how sorting is working, i.e. having two sort variables

            this._albums.Clear();

            foreach (var sortedAlbum in sortedAlbums)
                this._albums.Add(sortedAlbum);


            //force the listview to refresh the current page by rebinding the listviews datacontext
            this.VirtualAlbums = new VirtualizingCollection<Album>(_itemsProvider, 10);
        }

        private List<Album> SortBy<T>(Func<Album, T> selector, bool sortOrder)
        {
            List<Album> sortedAlbums = sortOrder
                                           ? this._albums.OrderBy(selector).ToList()
                                           : this._albums.OrderByDescending(selector).ToList();

            return sortedAlbums;
        }

        private void provider_ItemFinishedDownloading()
        {
            //Update totals after each of the albums details have been downloaded
            this.LinkedTotal = this._albums.Where(x => x.IsLinked == LinkStatus.Linked).Count();
            this.UnlinkedTotal = this._albums.Where(x => x.IsLinked == LinkStatus.Unlinked).Count();
            this.AlbumOrArtistMismatchTotal =
                this._albums.Where(x => x.IsLinked == LinkStatus.AlbumOrArtistMismatch).Count();
        }

        private void provider_AllDownloadsComplete()
        {
            this.CanSort = true;
        }
    }
}
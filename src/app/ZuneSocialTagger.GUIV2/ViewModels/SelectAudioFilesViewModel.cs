using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Caliburn.PresentationFramework;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using Album = ZuneSocialTagger.GUIV2.Models.Album;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using System.Xml;
using DbAlbumDetails = ZuneSocialTagger.GUIV2.Models.DbAlbumDetails;

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
        private List<Album> Albums { get; set; }
        private int _albumOrArtistMismatchTotal;
        private int _unlinkedTotal;
        private int _linkedTotal;
        private bool _isLoading;
        private readonly AlbumItemProvider _itemsProvider;

        private bool _canSort;

        public SelectAudioFilesViewModel(IUnityContainer container, IZuneWizardModel model, IZuneDatabaseReader dbReader)
        {
            _container = container;
            _model = model;
            _dbReader = dbReader;

            this.Albums = new List<Album>();

            this.LoadAlbumsFromZuneDatabase();

            _itemsProvider = new AlbumItemProvider(this.Albums);
            _itemsProvider.AllDownloadsComplete += provider_AllDownloadsComplete;
            _itemsProvider.ItemFinishedDownloading += provider_ItemFinishedDownloading;

            this.VirtualAlbums = new VirtualizingCollection<Album>(_itemsProvider, 10);

            //TODO: remove all zune fonts, default is close enough anyway
        }

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

        void provider_ItemFinishedDownloading()
        {
            this.LinkedTotal = this.Albums.Where(x => x.IsLinked == LinkStatus.Linked).Count();
            this.UnlinkedTotal = this.Albums.Where(x => x.IsLinked == LinkStatus.Unlinked).Count();
            this.AlbumOrArtistMismatchTotal = this.Albums.Where(x => x.IsLinked == LinkStatus.AlbumOrArtistMismatch).Count();
        }

        void provider_AllDownloadsComplete()
        {
            this.CanSort = true;
        }

        public void LoadFromZuneWebsite()
        {
            this.IsLoading = true;
            this.VirtualAlbums.LoadAllPages();
        }


        public void LoadAlbumsFromZuneDatabase()
        {
            foreach (var album in _dbReader.ReadAlbums())
            {
                this.Albums.Add(new Album    
                { 
                    ZuneAlbumMetaData = new DbAlbumDetails()
                        {
                            AlbumArtist = album.AlbumArtist,
                            AlbumTitle = album.AlbumTitle,
                            AlbumMediaId = album.AlbumMediaId,
                            ArtworkUrl = album.ArtworkUrl,
                            DateAdded = album.DateAdded
                        }
                });
            }

            foreach (var album in this.Albums)
                album.IsLinked = !String.IsNullOrEmpty(album.ZuneAlbumMetaData.AlbumMediaId)
                                     ? LinkStatus.Unknown
                                     : LinkStatus.Unlinked;
        }

        public void LinkAlbum(Album album)
        {
            _model.CurrentPage = _container.Resolve<SearchViewModel>();
        }

        public void Sort(string header)
        {
            var sortedAlbums = new List<Album>();

            if (header == "Album")
            {
                sortedAlbums = SortBy(x => x.ZuneAlbumMetaData.AlbumTitle,_albumSortOrder);
                _albumSortOrder = !_albumSortOrder;
            }

            if (header == "Linked To")
            {
                sortedAlbums = SortBy(x => x.IsLinked,_linkedToSortOrder);
                _linkedToSortOrder = !_linkedToSortOrder;
            }

            //TODO: dont like how sorting is working, i.e. having two sort variables

            this.Albums.Clear();

            foreach (var sortedAlbum in sortedAlbums)
                this.Albums.Add(sortedAlbum);


            //force the listview to refresh the current page by rebinding the listviews datacontext
            this.VirtualAlbums = new VirtualizingCollection<Album>(_itemsProvider, 10);
        }

        private List<Album> SortBy<T>(Func<Album, T> selector,bool sortOrder)
        {
            List<Album> sortedAlbums = sortOrder
                                           ? this.Albums.OrderBy(selector).ToList()
                                           : this.Albums.OrderByDescending(selector).ToList();

            return sortedAlbums;
        }
    }
}
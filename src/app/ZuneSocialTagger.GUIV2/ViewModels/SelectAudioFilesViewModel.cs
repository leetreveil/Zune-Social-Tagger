using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Caliburn.PresentationFramework;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Models;
using System.Linq;
using Album = ZuneSocialTagger.GUIV2.Models.Album;
using Screen = Caliburn.PresentationFramework.Screens.Screen;
using System.Xml;
using System.Diagnostics;
using System.Windows.Input;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SelectAudioFilesViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;
        private int _downloadProgress;
        private bool _sortOrder;
        private VirtualizingCollection<Album> _virtualAlbums;

        public VirtualizingCollection<Album> VirtualAlbums
        {
            get { return _virtualAlbums; }
            set
            {
                _virtualAlbums = value;
                NotifyOfPropertyChange(() => this.VirtualAlbums);
            }
        }

        private List<Album> Albums { get; set; }

        public int DownloadProgress
        {
            get { return _downloadProgress; }
            set
            {
                _downloadProgress = value;
                NotifyOfPropertyChange(() => this.DownloadProgress);
            }
        }

        public SelectAudioFilesViewModel(IUnityContainer container, IZuneWizardModel model)
        {
            _container = container;
            _model = model;

            this.Albums = new List<Album>();

            this.DownloadProgress = 0;

            _sortOrder = false;

            this.LoadAlbumsFromZuneDatabase();

            var provider = new AlbumItemProvider(this.Albums);

            this.VirtualAlbums = new VirtualizingCollection<Album>(provider, 10);

            this.VirtualAlbums.LoadPage(0);
            //TODO: remove all zune fonts, default is close enough anyway
        }

        public void LoadAlbumsFromZuneDatabase()
        {
            //load in albums from zune database
            //for testing we are going to use a xml doc

            //ThreadPool.QueueUserWorkItem(_ =>
            //     {
                     var xmlSerializer =
                         new XmlSerializer(typeof (BindableCollection<Album>));

                     var deserializedAlbums = (BindableCollection<Album>) 
                         xmlSerializer.Deserialize(XmlReader.Create(
                            new FileStream("zunedatabasecache.xml",FileMode.Open)));

                     List<Album> sortedAlbums =
                         deserializedAlbums.OrderBy(x => !x.IsLinked).ToList();

                     foreach (var album in sortedAlbums)
                     {
                         //TODO: using linq here

                         Album tbaAlbum = album;

                         tbaAlbum.IsLinked = !String.IsNullOrEmpty(album.AlbumMediaId);

                         this.Albums.Add(tbaAlbum);
                     }
        }

        public void LinkAlbum(Album album)
        {
            _model.CurrentPage = _container.Resolve<SearchViewModel>();
        }

        public void Sort(string header)
        {
            var sortedAlbums = new List<Album>();

            if (header == "Album")
                sortedAlbums = SortBy(x => x.ZuneAlbumMetaData.AlbumTitle);

            if (header == "Linked To")
                sortedAlbums = SortBy(x => !x.IsLinked);

            _sortOrder = !_sortOrder;

            this.Albums.Clear();

            foreach (var sortedAlbum in sortedAlbums)
                this.Albums.Add(sortedAlbum);


            //force the listview to refresh the current page by rebinding the listviews datacontext
            var provider = new AlbumItemProvider(this.Albums);
            this.VirtualAlbums = new VirtualizingCollection<Album>(provider, 10);
        }

        private List<Album> SortBy<T>(Func<Album, T> selector)
        {
            List<Album> sortedAlbums = _sortOrder
                                           ? this.Albums.OrderBy(selector).ToList()
                                           : this.Albums.OrderByDescending(selector).ToList();

            return sortedAlbums;
        }
    }
}
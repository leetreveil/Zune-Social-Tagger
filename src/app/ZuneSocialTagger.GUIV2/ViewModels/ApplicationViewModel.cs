using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using leetreveil.AutoUpdate.Framework;
using Ninject;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        #region DbLoadResult enum

        public enum DbLoadResult
        {
            Success,
            Failed
        }

        #endregion

        private readonly IKernel _container;
        private readonly IZuneWizardModel _model;
        private readonly CachedZuneDatabaseReader _cache;
        private readonly IZuneDatabaseReader _dbReader;
        private ViewModelBase _currentPage;
        private bool _updateAvailable;

        public ApplicationViewModel(IZuneWizardModel model, CachedZuneDatabaseReader cache, IZuneDatabaseReader dbReader,
                                    IKernel container)
        {
            _model = model;
            _cache = cache;
            _dbReader = dbReader;
            _container = container;

            this.ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            this.UpdateCommand = new RelayCommand(ShowUpdate);

            //register for changes to the current view model so we can switch between views
            Messenger.Default.Register<Type>(this, SwitchToView);

            //register for string messages, could be anything
            Messenger.Default.Register<string>(this, HandleMagicStrings);

            //register for error messages to be displayed
            Messenger.Default.Register<ErrorMessage>(this, DisplayErrorMessage);

            CheckForUpdates();
        }

        public void ApplicationViewHasLoaded()
        {
            if (Settings.Default.FirstViewToLoad == FirstViews.SelectAudioFilesViewModel)
            {
                _container.Bind<SelectAudioFilesViewModel>().ToSelf();
                this.CurrentPage = _container.Get<SelectAudioFilesViewModel>();
            }
            else
            {
                _container.Bind<WebAlbumListViewModel>().ToSelf().InSingletonScope();
                this.CurrentPage = _container.Get<WebAlbumListViewModel>();
            }

            InitDb();
        }

        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand ShowAboutSettingsCommand { get; private set; }

        public InlineZuneMessageViewModel InlineZuneMessage
        {
            get { return _container.Get<InlineZuneMessageViewModel>(); }
        }

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                _updateAvailable = value;
                RaisePropertyChanged("UpdateAvailable");
            }
        }

        public ViewModelBase CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                _currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }
        }

        private void InitDb()
        {
            if (_model.AlbumsFromDatabase.Count == 0)
            {
                DbLoadResult result = InitializeDatabase();

                //if we cannot load the database then switch to the other view
                if (result == DbLoadResult.Failed)
                {
                    var selectAudioFilesViewModel = _container.Get<SelectAudioFilesViewModel>();
                    selectAudioFilesViewModel.CanSwitchToNewMode = false;
                    this.CurrentPage = selectAudioFilesViewModel;
                    Settings.Default.FirstViewToLoad = FirstViews.SelectAudioFilesViewModel;
                }
                else
                {
                    ReadDatabase(false);
                }
            }
        }

        private static bool CheckIfZuneSoftwareIsRunning()
        {
            return Process.GetProcessesByName("Zune").Length != 0;
        }

        private void DisplayErrorMessage(ErrorMessage message)
        {
            UIDispatcher.GetDispatcher().Invoke(
                new Action(() => InlineZuneMessage.ShowMessage(message.ErrorMode, message.Message)));
        }

        private void HandleMagicStrings(string message)
        {
            if (message == "SWITCHTODB")
            {
                ReadDatabase(true);
            }
            else if (message == "ALBUMLINKED")
            {
                if (!CheckIfZuneSoftwareIsRunning())
                {
                    DisplayErrorMessage(new ErrorMessage(ErrorMode.Warning,
                                                         "Any albums you link / delink will not show their changes until the zune software is running."));
                }
            }
        }

        private void SwitchToView(Type viewType)
        {
            if (viewType == typeof (IFirstPage))
            {
                if (_currentPage.GetType() == typeof (WebAlbumListViewModel))
                {
                    this.CurrentPage = _container.Get<SelectAudioFilesViewModel>();
                    Settings.Default.FirstViewToLoad = FirstViews.SelectAudioFilesViewModel;
                }
                else if (_currentPage.GetType() == typeof (SelectAudioFilesViewModel))
                {
                    this.CurrentPage = _container.Get<WebAlbumListViewModel>();
                    Settings.Default.FirstViewToLoad = FirstViews.WebAlbumListViewModel;
                }
                else if (Settings.Default.FirstViewToLoad == FirstViews.WebAlbumListViewModel)
                {
                    this.CurrentPage = _container.Get<WebAlbumListViewModel>();
                }
                else if (Settings.Default.FirstViewToLoad == FirstViews.SelectAudioFilesViewModel)
                {
                    this.CurrentPage = _container.Get<SelectAudioFilesViewModel>();
                }
            }
            else
            {
                this.CurrentPage = (ViewModelBase) _container.Get(viewType);
            }
        }

        private DbLoadResult InitializeDatabase()
        {
            try
            {
                if (_dbReader.CanInitialize)
                {
                    //since the adapter is initally set to the cache this should always be true if the cache exists
                    bool initalized = _dbReader.Initialize();

                    if (!initalized)
                    {
                        this.InlineZuneMessage.ShowMessage(ErrorMode.Error, "Error loading zune database");
                        return DbLoadResult.Failed;
                    }

                    return DbLoadResult.Success;
                }

                this.InlineZuneMessage.ShowMessage(ErrorMode.Error,
                                                   "Failed to determine if the zune database can be loaded");
                return DbLoadResult.Failed;
            }
            catch (NotSupportedException e)
            {
                //if the version of the dll is not the version that this software supports
                //we should display an error but still attempt to load the database because it might work
                this.InlineZuneMessage.ShowMessage(ErrorMode.Error, e.Message);
                return DbLoadResult.Failed;
            }
            catch (FileNotFoundException e)
            {
                //if ZuneDBApi.dll cannot be found this will be thrown
                this.InlineZuneMessage.ShowMessage(ErrorMode.Error, e.Message);
                return DbLoadResult.Failed;
            }
            catch (Exception e)
            {
                this.InlineZuneMessage.ShowMessage(ErrorMode.Error, e.Message);
                return DbLoadResult.Failed;
            }
        }

        private void ReadDatabase(bool loadDatabase)
        {
            if (!loadDatabase)
            {
                //if we can read the cache then skip the database and just load that
                if (_cache.CanInitialize && _cache.Initialize())
                {
                    ThreadPool.QueueUserWorkItem(_ => {
                        foreach (AlbumDetails newAlbum in _cache.ReadAlbums())
                        {
                            AlbumDetails album = newAlbum;
                            UIDispatcher.GetDispatcher().Invoke(new Action(() =>
                                                                           _model.AlbumsFromDatabase.Add(
                                                                               new AlbumDetailsViewModel(album))));
                        }
                    });
                }
                else
                    ReadActualDatabase();
            }
            else
                ReadActualDatabase();
        }

        private void ReadActualDatabase()
        {
            ThreadPool.QueueUserWorkItem(_ => {
                foreach (Album newAlbum in _dbReader.ReadAlbums())
                {
                    Album album = newAlbum;
                    UIDispatcher.GetDispatcher().Invoke(new Action(() =>
                                                                   _model.AlbumsFromDatabase.Add(
                                                                       new AlbumDetailsViewModel(
                                                                           SharedMethods.ToAlbumDetails(album)))));
                }
            });
        }

        public void ShuttingDown()
        {
            WriteCacheToFile();
            Settings.Default.Save();
        }

        private void WriteCacheToFile()
        {
            List<AlbumDetailsXml> albums = (from album in _model.AlbumsFromDatabase
                                            select new AlbumDetailsXml
                                                       {
                                                           LinkStatus = album.LinkStatus,
                                                           WebAlbumMetaData =
                                                               AlbumXml.ToAlbumXml(album.WebAlbumMetaData),
                                                           ZuneAlbumMetaData =
                                                               AlbumXml.ToAlbumXml(album.ZuneAlbumMetaData),
                                                       }).ToList();
            if (albums.Count > 0)
            {
                try
                {
                    var xSer = new XmlSerializer(albums.GetType());

                    using (
                        var fs = new FileStream(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.xml"),
                                                FileMode.Create))
                        xSer.Serialize(fs, albums);
                }
                catch
                {
                }
            }
        }

        public void ShowUpdate()
        {
            new UpdateView().Show();
        }

        public void ShowAboutSettings()
        {
            new AboutView().Show();
        }

        private void CheckForUpdates()
        {
            string updaterPath = Path.Combine(Settings.Default.AppDataFolder, Settings.Default.UpdateExeName);

            if (Settings.Default.CheckForUpdates)
            {
                try
                {
                    //do update checking stuff here
                    UpdateManager updateManager = UpdateManager.Instance;

                    updateManager.UpdateExePath = updaterPath;
                    updateManager.AppFeedUrl = Settings.Default.UpdateFeedUrl;
                    updateManager.UpdateExe = Resources.socialtaggerupdater;

                    //always clean up at startup because we cant do it at the end
                    updateManager.CleanUp();

                    ThreadPool.QueueUserWorkItem(state => {
                        try
                        {
                            if (updateManager.CheckForUpdate())
                                UpdateAvailable = true;
                        }
                        catch
                        {
                        }
                    });
                }
                catch
                {
                    //TODO: log that we could not check for updates
                }
            }
        }
    }

    #region XmlSerializationStuff

    public class AlbumDetailsXml
    {
        public AlbumXml ZuneAlbumMetaData { get; set; }
        public AlbumXml WebAlbumMetaData { get; set; }
        public LinkStatus LinkStatus { get; set; }
    }

    public class AlbumXml
    {
        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }
        public string ArtworkUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public Guid AlbumMediaId { get; set; }
        public int MediaId { get; set; }
        public int ReleaseYear { get; set; }
        public int TrackCount { get; set; }
        public List<TrackXml> Tracks { get; set; }

        public static AlbumXml ToAlbumXml(Album album)
        {
            if (album != null)
            {
                return new AlbumXml
                           {
                               AlbumArtist = album.AlbumArtist,
                               AlbumMediaId = album.AlbumMediaId,
                               AlbumTitle = album.AlbumTitle,
                               ArtworkUrl = album.ArtworkUrl,
                               DateAdded = album.DateAdded,
                               MediaId = album.MediaId,
                               ReleaseYear = album.ReleaseYear,
                               TrackCount = album.TrackCount,
                               Tracks = album.Tracks != null ? album.Tracks.Select(TrackXml.ToTrackXml).ToList() : null
                           };
            }

            return null;
        }
    }

    public class TrackXml
    {
        public string FilePath { get; set; }
        public Guid MediaId { get; set; }

        public static TrackXml ToTrackXml(Track track)
        {
            return new TrackXml
                       {
                           FilePath = track.FilePath,
                           MediaId = track.MediaId
                       };
        }
    }

    #endregion
}
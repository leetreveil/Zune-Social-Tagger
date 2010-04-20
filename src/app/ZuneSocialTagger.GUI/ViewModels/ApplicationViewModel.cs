using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using leetreveil.AutoUpdate.Framework;
using Ninject;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.Views;

namespace ZuneSocialTagger.GUI.ViewModels
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
        private readonly Dispatcher _dispatcher;
        private readonly IZuneWizardModel _model;
        private readonly CachedZuneDatabaseReader _cache;
        private readonly IZuneDatabaseReader _dbReader;
        private ViewModelBase _currentPage;
        private bool _updateAvailable;
        private bool _shouldShowErrorMessage;
        private ErrorMode _errorMessageMode;
        private string _errorMessageText;

        public ApplicationViewModel(IZuneWizardModel model, CachedZuneDatabaseReader cache, IZuneDatabaseReader dbReader,
                                    IKernel container, Dispatcher dispatcher)
        {
            _model = model;
            _cache = cache;
            _dbReader = dbReader;
            _container = container;
            _dispatcher = dispatcher;

            this.ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            this.UpdateCommand = new RelayCommand(ShowUpdate);

            //register for changes to the current view model so we can switch between views
            Messenger.Default.Register<Type>(this, SwitchToView);

            //register for magic string messages, could be anything
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

        #region View Bindings

        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand ShowAboutSettingsCommand { get; private set; }

        public string ErrorMessageText
        {
            get { return _errorMessageText; }
            set
            {
                _errorMessageText = value;
                RaisePropertyChanged("ErrorMessageText");
            }
        }

        public ErrorMode ErrorMessageMode
        {
            get { return _errorMessageMode; }
            set
            {
                _errorMessageMode = value;
                RaisePropertyChanged("ErrorMessageMode");
            }
        }

        public bool ShouldShowErrorMessage
        {
            get { return _shouldShowErrorMessage; }
            set
            {
                _shouldShowErrorMessage = value;
                RaisePropertyChanged("ShouldShowErrorMessage");
            }
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

        #endregion

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
                    ReadCachedDatabase(result);
                }
            }
        }

        private static bool CheckIfZuneSoftwareIsRunning()
        {
            return Process.GetProcessesByName("Zune").Length != 0;
        }

        private void DisplayErrorMessage(ErrorMessage message)
        {
            this.ErrorMessageText = message.Message;
            this.ErrorMessageMode = message.ErrorMode;
            this.ShouldShowErrorMessage = true;
        }

        private void HandleMagicStrings(string message)
        {
            if (message == "SWITCHTODB")
            {
                ReadCachedDatabase(DbLoadResult.Failed);
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
                        DisplayErrorMessage(new ErrorMessage(ErrorMode.Error, "Error loading zune database"));
                        return DbLoadResult.Failed;
                    }

                    return DbLoadResult.Success;
                }

                DisplayErrorMessage(new ErrorMessage(ErrorMode.Error,
                                                     "Failed to determine if the zune database can be loaded"));
                return DbLoadResult.Failed;
            }
            catch (Exception e)
            {
                DisplayErrorMessage(new ErrorMessage(ErrorMode.Error, e.Message));
                return DbLoadResult.Failed;
            }
        }

        private void ReadCachedDatabase(DbLoadResult loadActualDatabase)
        {
            if (loadActualDatabase == DbLoadResult.Success)
            {
                //if we can read the cache then skip the database and just load that
                if (_cache.CanInitialize && _cache.Initialize())
                {
                    ThreadPool.QueueUserWorkItem(_ => {
                        foreach (AlbumDetails newAlbum in _cache.ReadAlbums())
                        {
                            AlbumDetails album = newAlbum;
                            _dispatcher.Invoke(new Action(() 
                                => _model.AlbumsFromDatabase.Add(new AlbumDetailsViewModel(album, _dbReader,_model))));
                        }

                        //after we have loaded the cache, we want to check for any new albums available in the database
                        GetNewOrRemovedAlbumsFromZuneDb();
                    });
                }
                else
                    ReadActualDatabase();
            }
            else
                ReadActualDatabase();
        }

        private void GetNewOrRemovedAlbumsFromZuneDb()
        {
            Dictionary<Album, DbAlbumChanged> updates =
                _dbReader.CheckForChanges(_model.AlbumsFromDatabase.Select(x => x.ZuneAlbumMetaData));

            if (updates.Count > 0)
            {
                //remove albums marked as to be removed
                var removed = updates.Where(x => x.Value == DbAlbumChanged.Removed);

                foreach (var removedAlbum in removed)
                {
                    KeyValuePair<Album, DbAlbumChanged> removedAlbumClosed = removedAlbum;

                    AlbumDetailsViewModel albumToBeRemoved =
                        _model.AlbumsFromDatabase.Where(
                            x => x.ZuneAlbumMetaData.MediaId == removedAlbumClosed.Key.MediaId).First();


                    _dispatcher.Invoke(
                        new Action(() => _model.AlbumsFromDatabase.Remove(albumToBeRemoved)));
                }

                //add new albums marked as new
                var addedUpdates = updates.Where(x => x.Value == DbAlbumChanged.Added);

                foreach (KeyValuePair<Album, DbAlbumChanged> newAlbum in addedUpdates)
                {
                    KeyValuePair<Album, DbAlbumChanged> album = newAlbum;

                    _dispatcher.Invoke(
                        new Action(
                            () =>
                            _model.AlbumsFromDatabase.Add(
                                new AlbumDetailsViewModel(SharedMethods.ToAlbumDetails(album.Key),_dbReader,_model))));
                }

                Messenger.Default.Send("SORT");

                if (addedUpdates.Count() > 0 && removed.Count() > 0)
                {
                    DisplayErrorMessage(new ErrorMessage(ErrorMode.Info,
                                                         String.Format(
                                                             "{0} albums were added and {1} albums were removed",
                                                             addedUpdates.Count(), removed.Count())));
                }
                else if (addedUpdates.Count() > 0)
                {
                    DisplayErrorMessage(new ErrorMessage(ErrorMode.Info,
                                                         String.Format("{0} albums were added",
                                                                       addedUpdates.Count())));
                }
                else if (removed.Count() > 0)
                {
                    DisplayErrorMessage(new ErrorMessage(ErrorMode.Info,
                                                         String.Format("{0} albums were removed",
                                                                       removed.Count())));
                }
            }
        }

        private void ReadActualDatabase()
        {
            ThreadPool.QueueUserWorkItem(_ => {
                foreach (Album newAlbum in _dbReader.ReadAlbums())
                {
                    Album album = newAlbum;
                    _dispatcher.Invoke(new Action(() => _model.AlbumsFromDatabase.Add(
                                                                       new AlbumDetailsViewModel(
                                                                           SharedMethods.ToAlbumDetails(album),_dbReader,_model))));
                }
            });
        }

        public void ApplicationIsShuttingDown()
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
            new UpdateView(new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version)).Show();
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
                               Tracks = album.Tracks != null ? album.Tracks.Select(x=> TrackXml.ToTrackXml(x)).ToList() : null
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
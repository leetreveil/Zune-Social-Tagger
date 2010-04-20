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

                DisplayErrorMessage(new ErrorMessage(ErrorMode.Error,"Error loading zune database"));
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
                        foreach (AlbumDetailsViewModel newAlbum in _cache.ReadAlbums())
                        {
                            var details = new AlbumDetailsViewModel(_dbReader, _model);
                            details.WebAlbumMetaData = newAlbum.WebAlbumMetaData;
                            details.ZuneAlbumMetaData = newAlbum.ZuneAlbumMetaData;
                            details.LinkStatus = newAlbum.LinkStatus;

                            _dispatcher.Invoke(new Action(() => _model.AlbumsFromDatabase.Add(details))); 

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
            var currentMediaIds = _model.AlbumsFromDatabase.Select(x => x.ZuneAlbumMetaData.MediaId);

            IEnumerable<Album> newAlbums = _dbReader.GetNewAlbums(currentMediaIds);
            IEnumerable<int> removedAlbums = _dbReader.GetRemovedAlbums(currentMediaIds).ToList();

            foreach (var album in newAlbums)
            {
                var albumDetails = new AlbumDetailsViewModel(_dbReader, _model);
                albumDetails.LinkStatus = album.AlbumMediaId.GetLinkStatusFromGuid();
                albumDetails.ZuneAlbumMetaData = album;

                _dispatcher.Invoke(new Action(() => _model.AlbumsFromDatabase.Add(albumDetails)));
            }

            foreach (var albumToBeRemoved in
                removedAlbums.Select(id => _model.AlbumsFromDatabase.Where(x => x.ZuneAlbumMetaData.MediaId == id).First()))
            {
                AlbumDetailsViewModel removed = albumToBeRemoved;
                _dispatcher.Invoke(new Action(() => _model.AlbumsFromDatabase.Remove(removed)));
            }

            //tell the WebAlbumListViewModel to sort its list and update
            Messenger.Default.Send("SORT");
            TellViewThatUpdatesHaveBeenAdded(newAlbums.Count(),removedAlbums.Count());
        }

        private void TellViewThatUpdatesHaveBeenAdded(int addedCount, int removedCount)
        {
                if (addedCount > 0 && removedCount > 0)
                {
                    DisplayErrorMessage(new ErrorMessage(ErrorMode.Info,
                                                         String.Format(
                                                             "{0} albums were added and {1} albums were removed",
                                                             addedCount, removedCount)));
                }
                else if (addedCount > 0)
                {
                    DisplayErrorMessage(new ErrorMessage(ErrorMode.Info,
                                                         String.Format("{0} albums were added",
                                                                       addedCount)));
                }
                else if (removedCount > 0)
                {
                    DisplayErrorMessage(new ErrorMessage(ErrorMode.Info,
                                                         String.Format("{0} albums were removed",
                                                                       removedCount)));
                }
        }

        private void ReadActualDatabase()
        {
            ThreadPool.QueueUserWorkItem(_ => {
                foreach (Album newAlbum in _dbReader.ReadAlbums())
                {
                    Album album = newAlbum;
                    var advm = new AlbumDetailsViewModel(_dbReader, _model);
                    advm.LinkStatus = album.AlbumMediaId.GetLinkStatusFromGuid();
                    advm.ZuneAlbumMetaData = album;

                    _dispatcher.Invoke(new Action(() => _model.AlbumsFromDatabase.Add(advm))); 
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
            var albums = _model.AlbumsFromDatabase;

            if (albums.Count > 0)
            {
                try
                {
                    var xSer = new XmlSerializer(albums.GetType());

                    using (var fs = new FileStream(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.xml"),
                                                FileMode.Create))
                        xSer.Serialize(fs, albums);
                }
                catch{}
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
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using leetreveil.AutoUpdate.Framework;
using Ninject;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.Views;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class ApplicationViewModel : ViewModelBaseExtended
    {
        private readonly IKernel _container;
        private readonly ZuneWizardModel _model;
        private readonly CachedZuneDatabaseReader _cache;
        private readonly IZuneDatabaseReader _dbReader;
        private ViewModelBaseExtended _currentPage;
        private bool _updateAvailable;
        private bool _shouldShowErrorMessage;
        private ErrorMode _errorMessageMode;
        private string _errorMessageText;
        private bool _loadWebView;
        private readonly ZuneObservableCollection<AlbumDetailsViewModel> _albums;

        public ApplicationViewModel(ZuneWizardModel model, CachedZuneDatabaseReader cache, IZuneDatabaseReader dbReader,
                                    IKernel container)
        {
            _model = model;
            _cache = cache;
            _dbReader = dbReader;
            _container = container;

            this.ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            this.UpdateCommand = new RelayCommand(ShowUpdate);
            this.CloseAppCommand = new RelayCommand(CloseApp);

            Application.Current.Exit += delegate { ApplicationIsShuttingDown(); };

            //register for changes to the current view model so we can switch between views
            Messenger.Default.Register<Type>(this, SwitchToView);

            //register for magic string messages, could be anything
            Messenger.Default.Register<string>(this, HandleMagicStrings);

            //register for error messages to be displayed
            Messenger.Default.Register<ErrorMessage>(this, DisplayMessage);

            _albums = new ZuneObservableCollection<AlbumDetailsViewModel>();
        }

        public void ViewHasLoaded()
        {
            InitDb();
            CheckForUpdates();
        }

        #region View Bindings

        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand ShowAboutSettingsCommand { get; private set; }
        public RelayCommand CloseAppCommand { get; private set; }

        public string ErrorMessageText
        {
            get { return _errorMessageText; }
            set
            {
                _errorMessageText = value;
                RaisePropertyChanged(() => this.ErrorMessageText);
            }
        }

        public ErrorMode ErrorMessageMode
        {
            get { return _errorMessageMode; }
            set
            {
                _errorMessageMode = value;
                RaisePropertyChanged(() => this.ErrorMessageMode);
            }
        }

        public bool ShouldShowErrorMessage
        {
            get { return _shouldShowErrorMessage; }
            set
            {
                _shouldShowErrorMessage = value;
                RaisePropertyChanged(() => this.ShouldShowErrorMessage);
            }
        }

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                _updateAvailable = value;
                RaisePropertyChanged(() => this.UpdateAvailable);
            }
        }

        public ViewModelBaseExtended CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                _currentPage = value;
                RaisePropertyChanged(() => this.CurrentPage);
            }
        }

        #endregion

        private void InitDb()
        {
            bool dbLoaded = InitializeDatabase();

            //if we cannot load the database then switch to the other view
            if (dbLoaded == false)
            {
                var selectAudioFilesViewModel = _container.Get<SelectAudioFilesViewModel>();
                selectAudioFilesViewModel.CanSwitchToNewMode = false;
                this.CurrentPage = selectAudioFilesViewModel;
                _loadWebView = false;
            }
            else
            {
                var viewModel = _container.Get<WebAlbumListViewModel>();
                viewModel.Albums = _albums;
                this.CurrentPage = viewModel;
                _loadWebView = true;
                ReadCachedDatabase(true);
            }
        }

        private static bool CheckIfZuneSoftwareIsRunning()
        {
            return Process.GetProcessesByName("Zune").Length != 0;
        }

        private void DisplayMessage(ErrorMessage message)
        {
            this.ErrorMessageText = message.Message;
            this.ErrorMessageMode = message.ErrorMode;
            this.ShouldShowErrorMessage = true;
        }

        private void HandleMagicStrings(string message)
        {
            if (message == "SWITCHTODB")
            {
                ReadActualDatabase();
            }
            else if (message == "ALBUMLINKED")
            {
                if (_loadWebView)
                {
                    if (!CheckIfZuneSoftwareIsRunning())
                    {
                        DisplayMessage(new ErrorMessage(ErrorMode.Warning,
                                                             "Any albums you link / delink will not show their changes until the zune software is running."));
                    }
                    else
                    {
                        GetNewOrRemovedAlbumsFromZuneDb();
                        _model.SelectedAlbum.AlbumDetails.RefreshAlbum();
                    }
                }
            }
            else if (message == "SWITCHTOFIRSTVIEW")
            {
                SwitchToView(_loadWebView ? typeof (WebAlbumListViewModel) : typeof (SelectAudioFilesViewModel));
            }
        }

        private void SwitchToView(Type viewType)
        {
            if (viewType == typeof(SelectAudioFilesViewModel))
            {
                _loadWebView = false;
            }
            if (viewType == typeof(WebAlbumListViewModel))
            {
                _loadWebView = true;
            }

           this.CurrentPage = (ViewModelBaseExtended)_container.Get(viewType);
        }

        private bool InitializeDatabase()
        {
            try
            {
                if (_dbReader.CanInitialize)
                {
                    bool initalized = _dbReader.Initialize();

                    if (!initalized)
                    {
                        DisplayMessage(new ErrorMessage(ErrorMode.Error, "Error loading zune database"));
                        return false;
                    }

                    return true;
                }

                DisplayMessage(new ErrorMessage(ErrorMode.Error, "Error loading zune database"));
                return false;
            }
            catch (Exception e)
            {
                DisplayMessage(new ErrorMessage(ErrorMode.Error, e.Message));
                return false;
            }
        }

        private void ReadCachedDatabase(bool dbInitialized)
        {
            if (dbInitialized)
                //if we can read the cache then skip the database and just load that
                if (_cache.CanInitialize && _cache.Initialize())
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        foreach (AlbumDetailsViewModel newAlbum in _cache.ReadAlbums())
                        {
                            var details = new AlbumDetailsViewModel(_dbReader, _model);
                            details.WebAlbumMetaData = newAlbum.WebAlbumMetaData;
                            details.ZuneAlbumMetaData = newAlbum.ZuneAlbumMetaData;
                            details.LinkStatus = newAlbum.LinkStatus;

                            DispatcherHelper.CheckBeginInvokeOnUI(() => _albums.Add(details));
                        }

                        //after we have loaded the cache, we want to check for any new albums available in the database
                        DispatcherHelper.CheckBeginInvokeOnUI(GetNewOrRemovedAlbumsFromZuneDb);
                    });
                }
                else
                    ReadActualDatabase();
        }

        private void GetNewOrRemovedAlbumsFromZuneDb()
        {
            var currentMediaIds = _albums.Select(x => x.ZuneAlbumMetaData.MediaId);

            IEnumerable<Album> newAlbums = _dbReader.GetNewAlbums(currentMediaIds).ToList();
            IEnumerable<int> removedAlbums = _dbReader.GetRemovedAlbums(currentMediaIds).ToList();

            foreach (var album in newAlbums)
            {
                var albumDetails = new AlbumDetailsViewModel(_dbReader, _model);
                albumDetails.LinkStatus = album.AlbumMediaId.GetLinkStatusFromGuid();
                albumDetails.ZuneAlbumMetaData = album;

                _albums.Add(albumDetails);
            }

            foreach (var albumToBeRemoved in
                removedAlbums.Select(id => _albums.Where(x => x.ZuneAlbumMetaData.MediaId == id).First()))
            {
                AlbumDetailsViewModel toBeRemoved = albumToBeRemoved;
                _albums.Remove(toBeRemoved);
            }

            //tell the WebAlbumListViewModel to sort its list
            Messenger.Default.Send("SORT");
            TellViewThatUpdatesHaveBeenAdded(newAlbums.Count(), removedAlbums.Count());
        }

        private void TellViewThatUpdatesHaveBeenAdded(int addedCount, int removedCount)
        {
            if (addedCount > 0 && removedCount > 0)
            {
                DisplayMessage(new ErrorMessage(ErrorMode.Info,
                                                     String.Format(
                                                         "{0} albums were added and {1} albums were removed",
                                                         addedCount, removedCount)));
            }
            else if (addedCount > 0)
            {
                DisplayMessage(new ErrorMessage(ErrorMode.Info,
                                                     String.Format("{0} albums were added",
                                                                   addedCount)));
            }
            else if (removedCount > 0)
            {
                DisplayMessage(new ErrorMessage(ErrorMode.Info,
                                                     String.Format("{0} albums were removed",
                                                                   removedCount)));
            }
        }

        private void ReadActualDatabase()
        {
            _albums.Clear();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (Album newAlbum in _dbReader.ReadAlbums())
                {
                    Album album = newAlbum;
                    var advm = new AlbumDetailsViewModel(_dbReader, _model);
                    advm.LinkStatus = album.AlbumMediaId.GetLinkStatusFromGuid();
                    advm.ZuneAlbumMetaData = album;

                    DispatcherHelper.CheckBeginInvokeOnUI(() => _albums.Add(advm));
                }
            });
        }

        private void CloseApp()
        {
            Application.Current.Shutdown();
        }

        private void ApplicationIsShuttingDown()
        {
            WriteCacheToFile();
            Settings.Default.Save();
        }

        private void WriteCacheToFile()
        {
            if (_albums.Count > 0)
            {
                var xSer = new XmlSerializer(_albums.GetType());

                using (var fs = new FileStream(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.xml"),
                                            FileMode.Create))
                    xSer.Serialize(fs, _albums);
            }
        }

        private void ShowUpdate()
        {
            var updateViewModel = new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version);
            var updateView = new UpdateView {DataContext = updateViewModel};
            updateView.Show();
        }

        private void ShowAboutSettings()
        {
            new AboutView{DataContext = new AboutViewModel()}.Show();
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

                    ThreadPool.QueueUserWorkItem(state =>
                    {
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
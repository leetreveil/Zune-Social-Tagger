using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using ZuneSocialTagger.GUI.ViewsViewModels.About;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.Update;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{
    public class ApplicationViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly SelectAudioFilesViewModel _selectAudioFilesViewModel;
        private readonly WebAlbumListViewModel _webAlbumListViewModel;
        private readonly ZuneObservableCollection<AlbumDetailsViewModel> _albums;
        private ViewModelBase _currentPage;
        private bool _updateAvailable;
        private bool _shouldShowErrorMessage;
        private ErrorMode _errorMessageMode;
        private string _errorMessageText;
        private bool _loadWebView;

        public static ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public static ExpandedAlbumDetailsViewModel AlbumDetailsFromWeb { get; set; }

        /// <summary>
        /// The downloaded album songs after searching
        /// </summary>
        public static List<WebTrack> SongsFromWebsite { get; set; }

        /// <summary>
        /// The actual track details from the audio files
        /// </summary>
        public static List<Song> SongsFromFile { get; set; }

        public ApplicationViewModel(IZuneDatabaseReader dbReader,
                                    SelectAudioFilesViewModel selectAudioFilesViewModel,
                                    WebAlbumListViewModel webAlbumListViewModel)
        {
            _dbReader = dbReader;
            _selectAudioFilesViewModel = selectAudioFilesViewModel;
            _webAlbumListViewModel = webAlbumListViewModel;

            this.ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            this.UpdateCommand = new RelayCommand(ShowUpdate);
            this.CloseAppCommand = new RelayCommand(CloseApp);

            System.Windows.Application.Current.Exit += delegate { ApplicationIsShuttingDown(); };

            //register for changes to the current view model so we can switch between views
            Messenger.Default.Register<Type>(this, SwitchToView);

            //register for magic string messages, could be anything
            Messenger.Default.Register<string>(this, HandleMagicStrings);

            //register for error messages to be displayed
            Messenger.Default.Register<ErrorMessage>(this, DisplayMessage);

            _albums = new ZuneObservableCollection<AlbumDetailsViewModel>();

            _dbReader.FinishedReadingAlbums += _dbReader_FinishedReadingAlbums;
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

        public ViewModelBase CurrentPage
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
                _selectAudioFilesViewModel.CanSwitchToNewMode = false;
                this.CurrentPage = _selectAudioFilesViewModel;
                _loadWebView = false;
            }
            else
            {
                _webAlbumListViewModel.Albums = _albums;
                this.CurrentPage = _webAlbumListViewModel;
                _loadWebView = true;
                ReadCachedDatabase();
            }
        }

        private void DisplayMessage(ErrorMessage message)
        {
            this.ErrorMessageText = message.Message;
            this.ErrorMessageMode = message.ErrorMode;
            this.ShouldShowErrorMessage = true;
        }

        private void HandleMagicStrings(string message)
        {
            if (message == "SORT")
            {
                SortData(Settings.Default.SortOrder);
            }
            if (message == "SWITCHTODB")
            {
                ReadActualDatabase();
            }
            else if (message == "ALBUMLINKED")
            {
                if (_loadWebView)
                {
                    if (!SharedMethods.CheckIfZuneSoftwareIsRunning())
                    {
                        DisplayMessage(new ErrorMessage(ErrorMode.Warning,
                                                        "Any albums you link / delink will not show their changes until the zune software is running."));
                    }
                    else
                    {
                        GetNewOrRemovedAlbumsFromZuneDb();
                        Messenger.Default.Send<string, WebAlbumListViewModel>("REFRESHCURRENTALBUM");
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
            this.CurrentPage = App.GetViewForType(viewType);

            //each time we switch to the two starting views we need to remember which one to go back to if the
            //user goes back through the wizard, this is just at runtime it is not remembered at startup
            if (this.CurrentPage.GetType() == typeof (SelectAudioFilesViewModel))
                _loadWebView = false;

            if (this.CurrentPage.GetType() == typeof (WebAlbumListViewModel))
                _loadWebView = true;
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

        private void ReadCachedDatabase()
        {
            string filePath = Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.dat");

            ThreadPool.QueueUserWorkItem(_ => {
                FileStream fs;
                try
                {
                    fs = new FileStream(filePath, FileMode.Open);
                }
                catch (Exception)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(ReadActualDatabase);
                    return;
                }

                try
                {
                    var binaryFormatter = new BinaryFormatter();
                    var deserialized = (IEnumerable<AlbumDetailsViewModel>) binaryFormatter.Deserialize(fs);

                    DispatcherHelper.CheckBeginInvokeOnUI(() => {
                        foreach (var album in deserialized)
                        {
                            _albums.Add(new AlbumDetailsViewModel(_dbReader)
                                            {
                                                WebAlbumMetaData = album.WebAlbumMetaData,
                                                ZuneAlbumMetaData = album.ZuneAlbumMetaData,
                                                LinkStatus = album.LinkStatus
                                            });
                        }

                        GetNewOrRemovedAlbumsFromZuneDb();

                        Messenger.Default.Send<string,WebAlbumListViewModel>("FINISHEDLOADING");
                    });
                }
                catch (SerializationException)
                {
                    //TODO: log error
                    ReadActualDatabase();
                }
                finally
                {
                    fs.Close();
                }
            });
        }

        private void GetNewOrRemovedAlbumsFromZuneDb()
        {
            ThreadPool.QueueUserWorkItem(_ => {
                var currentMediaIds = _albums.Select(x => x.ZuneAlbumMetaData.MediaId);

                var newAlbums = _dbReader.GetNewAlbums(currentMediaIds).ToList();
                var removedAlbums = _dbReader.GetRemovedAlbums(currentMediaIds).ToList();

                DispatcherHelper.CheckBeginInvokeOnUI(() => {
                    foreach (var album in newAlbums)
                    {
                        var albumDetails = new AlbumDetailsViewModel(_dbReader)
                                               {
                                                   LinkStatus = album.AlbumMediaId.GetLinkStatusFromGuid(),
                                                   ZuneAlbumMetaData = album
                                               };

                        _albums.Add(albumDetails);
                    }

                    foreach (var albumToBeRemoved in
                        removedAlbums.Select(id => _albums.Where(x => x.ZuneAlbumMetaData.MediaId == id).First()))
                    {
                        AlbumDetailsViewModel toBeRemoved = albumToBeRemoved;
                        _albums.Remove(toBeRemoved);
                    }

                    if (newAlbums.Count > 0 || removedAlbums.Count > 0)
                    {
                        //tell the WebAlbumListViewModel to sort its list because there may be new items
                        SortData(Settings.Default.SortOrder);

                        TellViewThatUpdatesHaveBeenAdded(newAlbums.Count(), removedAlbums.Count());
                    }
                });
            });
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

            ThreadPool.QueueUserWorkItem(_ => {
                foreach (DbAlbum newAlbum in _dbReader.ReadAlbums())
                {
                    var advm = new AlbumDetailsViewModel(_dbReader)
                                   {
                                       LinkStatus = newAlbum.AlbumMediaId.GetLinkStatusFromGuid(),
                                       ZuneAlbumMetaData = newAlbum
                                   };

                    DispatcherHelper.CheckBeginInvokeOnUI(() => _albums.Add(advm));
                }

                Messenger.Default.Send<string, WebAlbumListViewModel>("FINISHEDLOADING");
            });
        }

        private static void CloseApp()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ApplicationIsShuttingDown()
        {
            WriteCacheToFile();
            Settings.Default.Save();
        }

        private void WriteCacheToFile()
        {
            var fs = new FileStream(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.dat"), FileMode.Create);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fs, _albums);
            fs.Close();
            //try
            //{

            //}
            //catch (SerializationException e)
            //{
            //    Debug.WriteLine(e);
            //}
            //finally
            //{

            //}
        }

        private static void ShowUpdate()
        {
            var updateViewModel = new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version);
            var updateView = new UpdateView {DataContext = updateViewModel};
            updateView.Show();
        }

        private static void ShowAboutSettings()
        {
            new AboutView {DataContext = new AboutViewModel()}.Show();
        }

        private void CheckForUpdates()
        {
            string updaterPath = Path.Combine(Settings.Default.AppDataFolder, Settings.Default.UpdateExeName);

            if (Settings.Default.CheckForUpdates)
            {
                //do update checking stuff here
                UpdateManager updateManager = UpdateManager.Instance;

                updateManager.UpdateExePath = updaterPath;
                updateManager.AppFeedUrl = Settings.Default.UpdateFeedUrl;
                updateManager.UpdateExe = Resources.socialtaggerupdater;

                try
                {
                    //always clean up at startup because we cant do it at the end
                    updateManager.CleanUp();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                ThreadPool.QueueUserWorkItem(state => {
                    try
                    {
                        if (updateManager.CheckForUpdate())
                            UpdateAvailable = true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                });
            }
        }

        private void SortData(SortOrder sortOrder)
        {
            Settings.Default.SortOrder = sortOrder;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                switch (sortOrder)
                {
                    case SortOrder.DateAdded:
                        _albums.SortDesc(x => x.ZuneAlbumMetaData.DateAdded);
                        break;
                    case SortOrder.Album:
                        _albums.Sort(x => x.ZuneAlbumMetaData.Title);
                        break;
                    case SortOrder.Artist:
                        _albums.Sort(x => x.ZuneAlbumMetaData.Artist);
                        break;
                    case SortOrder.LinkStatus:
                        _albums.Sort(x => x.LinkStatus);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        void _dbReader_FinishedReadingAlbums()
        {
            SortData(Settings.Default.SortOrder);
        }
    }
}
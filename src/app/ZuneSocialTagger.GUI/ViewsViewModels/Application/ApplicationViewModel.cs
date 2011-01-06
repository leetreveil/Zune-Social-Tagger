using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using ZuneSocialTagger.GUI.ViewsViewModels.About;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.Update;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;
using ProtoBuf;
using System.ComponentModel;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{

    public class ApplicationViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly SafeObservableCollection<AlbumDetailsViewModel> _albums;
        private readonly IViewLocator _locator;
        private bool _updateAvailable;
        private bool _shouldShowErrorMessage;
        private ErrorMode _errorMessageMode;
        private string _errorMessageText;
        private WebAlbumListViewModel _webAlbumListViewModel;


        public ApplicationViewModel(IZuneDatabaseReader dbReader,
                                    SafeObservableCollection<AlbumDetailsViewModel> albums,
                                    IViewLocator locator)
        {
            _dbReader = dbReader;

            this.ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            this.UpdateCommand = new RelayCommand(ShowUpdate);
            this.CloseAppCommand = new RelayCommand(CloseApp);

            System.Windows.Application.Current.Exit += delegate { ApplicationIsShuttingDown(); };

            //register for error messages to be displayed
            Messenger.Default.Register<ErrorMessage>(this, this.Messages.Add);

            _albums = albums;
            _locator = locator;
            locator.SwitchToViewRequested += view => {
                this.CurrentPage = view;
            };
        }

        public void ViewHasLoaded()
        {
            CheckForUpdates();
            InitDb();
        }

        #region View Bindings

        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand ShowAboutSettingsCommand { get; private set; }
        public RelayCommand CloseAppCommand { get; private set; }

        private ObservableCollection<ErrorMessage> _messages;
        public ObservableCollection<ErrorMessage> Messages 
        {
            get 
            {
                if (_messages == null) {
                    _messages = new ObservableCollection<ErrorMessage>();
                    RaisePropertyChanged(() => this.Messages);
                }
                return _messages; 
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

        private UserControl _currentPage;
        public UserControl CurrentPage
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
            if (dbLoaded)
            {
                _webAlbumListViewModel = _locator.SwitchToView<WebAlbumListView,WebAlbumListViewModel>();
                ReadCachedDatabase();
            }
            else
            {
                var selectAudioFilesViewModel = _locator.SwitchToView<SelectAudioFilesView,SelectAudioFilesViewModel>();
                selectAudioFilesViewModel.CanSwitchToNewMode = false;
            }
        }

        public void AlbumBeenLinked()
        {
            if (this.CurrentPage.GetType() == typeof(WebAlbumListViewModel))
            {
                if (!SharedMethods.CheckIfZuneSoftwareIsRunning())
                {
                    this.Messages.Add(new ErrorMessage(ErrorMode.Warning,
                                                    "Any albums you link / delink will not show their changes until the zune software is running."));
                }
                else
                {
                    GetNewOrRemovedAlbumsFromZuneDb();
                    _webAlbumListViewModel.SelectedAlbum.RefreshAlbum();
                }
            }
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
                        this.Messages.Add(new ErrorMessage(ErrorMode.Error, "Error loading zune database"));
                        return false;
                    }

                    return true;
                }

                this.Messages.Add(new ErrorMessage(ErrorMode.Error, "Error loading zune database"));
                return false;
            }
            catch (Exception e)
            {
                this.Messages.Add(new ErrorMessage(ErrorMode.Error, e.Message));
                return false;
            }
        }

        private void ReadCachedDatabase()
        {
            try
            {
                List<AlbumDetailsViewModel> deserialized = null;
                using (var fs = new FileStream(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache2.dat"), FileMode.Open))
                {
                    deserialized = Serializer.Deserialize<List<AlbumDetailsViewModel>>(fs);
                }

                if (deserialized == null) throw new NullReferenceException();
                if (deserialized.Count() == 0) ReadActualDatabase();

                foreach (var album in deserialized)
                {
                    var albumDetailsViewModel = _locator.Resolve<AlbumDetailsViewModel>();
                    albumDetailsViewModel.WebAlbumMetaData = album.WebAlbumMetaData;
                    albumDetailsViewModel.ZuneAlbumMetaData = album.ZuneAlbumMetaData;
                    albumDetailsViewModel.LinkStatus = album.LinkStatus;

                    _albums.Add(albumDetailsViewModel);
                }

                GetNewOrRemovedAlbumsFromZuneDb();

                _webAlbumListViewModel.DataHasLoaded();

            }
            catch (Exception)
            {
                ReadActualDatabase();
            }
        }

        private void GetNewOrRemovedAlbumsFromZuneDb()
        {
            ThreadPool.QueueUserWorkItem(_ => {
                var currentMediaIds = _albums.Select(x => x.ZuneAlbumMetaData.MediaId).ToList();

                var newAlbums = _dbReader.GetNewAlbums(currentMediaIds).ToList();
                var removedAlbums = _dbReader.GetRemovedAlbums(currentMediaIds).ToList();

                if (newAlbums.Count > 0 || removedAlbums.Count > 0)
                {
                    foreach (var album in newAlbums)
                    {
                        var albumDetailsViewModel = _locator.Resolve<AlbumDetailsViewModel>();
                        albumDetailsViewModel.LinkStatus = album.AlbumMediaId.GetLinkStatusFromGuid();
                        albumDetailsViewModel.ZuneAlbumMetaData = album;
                        _albums.Add(albumDetailsViewModel);
                    }

                    foreach (var albumToBeRemoved in
                        removedAlbums.Select(id => _albums.Where(x => x.ZuneAlbumMetaData.MediaId == id).First()))
                    {
                        AlbumDetailsViewModel toBeRemoved = albumToBeRemoved;
                        _albums.Remove(toBeRemoved);
                    }

                    _webAlbumListViewModel.Sort();
                    TellViewThatUpdatesHaveBeenAdded(newAlbums.Count(), removedAlbums.Count());
                }
            });
        }

        private void TellViewThatUpdatesHaveBeenAdded(int addedCount, int removedCount)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                this.Messages.Add(new ErrorMessage(ErrorMode.Warning, "fffffffffffffffffffffffuuuuuuuuuuuuck"));


                if (addedCount > 0 && removedCount > 0)
                {
                    this.Messages.Add(new ErrorMessage(ErrorMode.Info,
                                                    String.Format(
                                                        "{0} albums were added and {1} albums were removed",
                                                        addedCount, removedCount)));
                }
                else if (addedCount > 0)
                {
                    this.Messages.Add(new ErrorMessage(ErrorMode.Info,
                                                    String.Format("{0} albums were added",
                                                                  addedCount)));
                }
                else if (removedCount > 0)
                {
                    this.Messages.Add(new ErrorMessage(ErrorMode.Info,
                                                    String.Format("{0} albums were removed",
                                                                  removedCount)));
                }
            });

        }

        public void ReadActualDatabase()
        {
            _albums.Clear();
            _webAlbumListViewModel.SuspendSorting();
            _webAlbumListViewModel.ResetSortOrder();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (DbAlbum newAlbum in _dbReader.ReadAlbums())
                {
                    var albumDetailsViewModel = _locator.Resolve<AlbumDetailsViewModel>();
                    albumDetailsViewModel.LinkStatus = newAlbum.AlbumMediaId.GetLinkStatusFromGuid();
                    albumDetailsViewModel.ZuneAlbumMetaData = newAlbum;

                    _albums.Add(albumDetailsViewModel);
                }
                _webAlbumListViewModel.DataHasLoaded();
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
            using (var file = File.Create(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache2.dat"))) 
            {
                Serializer.Serialize(file, _albums.ToList());
            }
        }


        private static void ShowUpdate()
        {
            var updateViewModel = new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version);
            var updateView = new UpdateView {DataContext = updateViewModel};
            updateView.Show();
        }

        private void ShowAboutSettings()
        {
            new AboutView {DataContext = _locator.Resolve<AboutViewModel>()}.Show();
        }

        private void CheckForUpdates()
        {
            if (Settings.Default.CheckForUpdates)
            {
                //do update checking stuff here
                UpdateManager updateManager = UpdateManager.Instance;

                updateManager.UpdateExePath = Path.Combine(Settings.Default.AppDataFolder, Settings.Default.UpdateExeName);
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
    }
}
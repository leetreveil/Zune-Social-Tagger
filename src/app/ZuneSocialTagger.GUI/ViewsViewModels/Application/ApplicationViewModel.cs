using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using System.Diagnostics;
using ZuneSocialTagger.GUI.ViewsViewModels.About;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.Update;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;
using ProtoBuf;
using System.Windows.Input;
using SortOrder = ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList.SortOrder;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Application
{

    public class ApplicationViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly SafeObservableCollection<AlbumDetailsViewModel> _albums;
        private readonly IViewLocator _viewLocator;
        private WebAlbumListViewModel _webAlbumListViewModel;
        private List<MinCache> _cache;

        public ApplicationViewModel(IZuneDatabaseReader dbReader,
                                    SafeObservableCollection<AlbumDetailsViewModel> albums,
                                    IViewLocator locator)
        {
            _dbReader = dbReader;
            _albums = albums;
            _viewLocator = locator;

            //register for notification messages
            Messenger.Default.Register<ErrorMessage>(this, Notifications.Add);

            locator.SwitchToViewRequested += view => {
                CurrentPage = view;
            };
        }

        public void ViewHasLoaded()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Initialize();
                ReadCache();
                CheckForUpdates();
                CheckLocale();
            });
        }

        #region View Bindings

        private ICommand _updateCommand;
        public ICommand UpdateCommand 
        {
            get
            {
                if (_updateCommand == null)
                    _updateCommand = new RelayCommand(ShowUpdate);

                return _updateCommand;
            }
        }

        private ICommand _showAboutSettingsCommand;
        public ICommand ShowAboutSettingsCommand
        {
            get
            {
                if (_showAboutSettingsCommand == null)
                    _showAboutSettingsCommand = new RelayCommand(ShowAboutSettings);

                return _showAboutSettingsCommand;
            }
        }

        private ICommand _closeAppCommand;
        public ICommand CloseAppCommand
        {
            get
            {
                if (_closeAppCommand == null)
                    _closeAppCommand = new RelayCommand(CloseApp);

                return _closeAppCommand;
            }
        }

        private SafeObservableCollection<ErrorMessage> _notifications;
        public SafeObservableCollection<ErrorMessage> Notifications 
        {
            get 
            {
                if (_notifications == null) {
                    _notifications = new SafeObservableCollection<ErrorMessage>();
                    RaisePropertyChanged(() => Notifications);
                }
                return _notifications; 
            }
        }

        private bool _updateAvailable;
        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                _updateAvailable = value;
                RaisePropertyChanged(() => UpdateAvailable);
            }
        }

        private UserControl _currentPage;
        public UserControl CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                _currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
            }
        }

        #endregion

        private void Initialize()
        {
            if (InitializeDatabase())
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    _webAlbumListViewModel = _viewLocator.SwitchToView<WebAlbumListView, WebAlbumListViewModel>();
                });
                
                ReadActualDatabase();
            }
            //if we cannot interop with the zune database switch to the old view
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var selectAudioFilesViewModel = _viewLocator.SwitchToView<SelectAudioFilesView, SelectAudioFilesViewModel>();
                    selectAudioFilesViewModel.CanSwitchToNewMode = false;
                });
            }
        }

        public void AlbumBeenLinked()
        {
            if (CurrentPage.GetType() == typeof(WebAlbumListViewModel))
            {
                if (!SharedMethods.CheckIfZuneSoftwareIsRunning())
                {
                    Notifications.Add(new ErrorMessage(ErrorMode.Warning,
                                                    "Any albums you link / delink will not show their changes until the zune software is running."));
                }
                else
                {
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
                        Notifications.Add(new ErrorMessage(ErrorMode.Error, "Error loading zune database"));
                        return false;
                    }

                    return true;
                }

                Notifications.Add(new ErrorMessage(ErrorMode.Error, "Error loading zune database"));
                return false;
            }
            catch (Exception e)
            {
                Notifications.Add(new ErrorMessage(ErrorMode.Error, e.Message));
                return false;
            }
        }

        private  void ReadCache()
        {
            try
            {
                using (var file = File.Open(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache3.dat"), FileMode.Open))
                {
                    _cache = Serializer.Deserialize<List<MinCache>>(file);
                }
            }
            catch
            {}
        }

        private void WriteCache()
        {
            using (var file = File.Create(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache3.dat")))
            {
                Serializer.Serialize(file, _albums.Select(x=> new MinCache{MediaId = x.MediaId, Right = x.Right}).ToList());
            }
        }

        public void ReadActualDatabase()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Core.ZuneDatabase.SortOrder so;
                switch (Settings.Default.SortOrder)
                {
                    case SortOrder.DateAdded:
                        so = Core.ZuneDatabase.SortOrder.DateAdded;
                        break;
                    case SortOrder.Album:
                        so = Core.ZuneDatabase.SortOrder.Album;
                        break;
                    case SortOrder.Artist:
                        so = Core.ZuneDatabase.SortOrder.Artist;
                        break;
                    default:
                        so = Core.ZuneDatabase.SortOrder.DateAdded;
                        break;
                }
                foreach (DbAlbum newAlbum in _dbReader.ReadAlbums(so))
                {
                    var newalbumDetails = _viewLocator.Resolve<AlbumDetailsViewModel>();
                    newalbumDetails.LinkStatus = newAlbum.AlbumMediaId.GetLinkStatusFromGuid();
                    newalbumDetails.DateAdded = newAlbum.DateAdded;
                    newalbumDetails.MediaId = newAlbum.MediaId;
                    newalbumDetails.Left = new AlbumThumbDetails
                    {
                        Artist = newAlbum.Artist,
                        ArtworkUrl = newAlbum.ArtworkUrl,
                        Title = newAlbum.Title,
                    };

                    if (newalbumDetails.LinkStatus == LinkStatus.Unknown)
                    {
                        if (_cache != null)
                        {
                            DbAlbum album = newAlbum;
                            var cachedObject = _cache.Find(x => x.MediaId == album.MediaId);

                            if (cachedObject.Right != null)
                            {
                                newalbumDetails.Right = new AlbumThumbDetails
                                                            {
                                    Artist = cachedObject.Right.Artist,
                                    ArtworkUrl = cachedObject.Right.ArtworkUrl,
                                    Title = cachedObject.Right.Title
                                };


                                newalbumDetails.LinkStatus = SharedMethods.GetAlbumLinkStatus(
                                    newalbumDetails.Left.Title,
                                    newalbumDetails.Left.Artist,
                                    newalbumDetails.Right.Title,
                                    newalbumDetails.Right.Artist);
                            }
                        }
                    }

                    _albums.Add(newalbumDetails);
                }

                _webAlbumListViewModel.Sort();
            });
        }

        private void CheckLocale()
        {
            var locale = Thread.CurrentThread.CurrentCulture.Name;
            LocaleDownloader.IsMarketPlaceEnabledForLocaleAsync(locale, isEnabled =>
            {
                if (!isEnabled)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var msg = String.Format("The Zune Marketplace is not yet available in your region ({0}). You" +
                            " may not get any search results when trying to link an album to the marketplace.", locale);
                        Notifications.Add(new ErrorMessage(ErrorMode.Info, msg));
                    });
                }
            });
        }

        private static void CloseApp()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void ApplicationIsShuttingDown()
        {
            WriteCache();
            Settings.Default.Save();
        }

        private static void ShowUpdate()
        {
            var updateViewModel = new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version);
            var updateView = new UpdateView {DataContext = updateViewModel};
            updateView.Show();
        }

        private void ShowAboutSettings()
        {
            new AboutView {DataContext = _viewLocator.Resolve<AboutViewModel>()}.Show();
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using leetreveil.AutoUpdate.Framework;
using Ninject;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;
using System.Diagnostics;


namespace ZuneSocialTagger.GUIV2
{
    public class ApplicationModel : ViewModelBase
    {
        private IZuneDbAdapter _adapter;
        private readonly IKernel _container;
        private bool _updateAvailable;
        private ViewModelBase _currentPage;
        private readonly IZuneWizardModel _model;

        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand ShowAboutSettingsCommand { get; private set; }
        public InlineZuneMessageViewModel InlineZuneMessage { get; set; }

        public ApplicationModel(IZuneWizardModel model, IZuneDbAdapter adapter, IKernel container)
        {
            _model = model;
            _adapter = adapter;
            _container = container;

            CheckForUpdates();
            SetupCommandBindings();

            //register for changes to the current view model so we can switch between views
            Messenger.Default.Register<Type>(this, SwitchToView);

            //register for database switch messages
            Messenger.Default.Register<string>(this, SwitchToDatabase);

            //register for error messages to be displayed
            Messenger.Default.Register<ErrorMessage>(this, DisplayErrorMessage);

            this.InlineZuneMessage = new InlineZuneMessageViewModel();

            if (Settings.Default.FirstViewToLoad == FirstViews.SelectAudioFilesViewModel)
                ShowSelectAudioFilesView();
            else
            {
                this.CurrentPage = _container.Get<WebAlbumListViewModel>();
            }
        }

        private void DisplayErrorMessage(ErrorMessage message)
        {
            this.InlineZuneMessage.ShowMessage(message.ErrorMode, message.Message);
        }

        private void SwitchToDatabase(string message)
        {
            if (message == "SWITCHTODB")
            {
                _container.Rebind<IZuneDbAdapter>().To<ZuneDbAdapter>().InSingletonScope();
                _adapter = _container.Get<IZuneDbAdapter>();

                this.CurrentPage = _container.Get<WebAlbumListViewModel>();
            }
        }

        private void SwitchToView(Type viewType)
        {
            this.CurrentPage = (ViewModelBase) _container.Get(viewType);
        }

        private void SetupCommandBindings()
        {
            this.ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            this.UpdateCommand = new RelayCommand(ShowUpdate);
        }

        public enum DbLoadResult
        {
            Success,
            Failed
        }

        private DbLoadResult InitializeDatabase()
        {
            try
            {
                if (_adapter.CanInitialize)
                {
                    //since the adapter is initally set to the cache this should always be true if the cache exists
                    bool initalized = _adapter.Initialize();

                    if (!initalized)
                    {
                        //fall back to the actual zune database if the cache could not be loaded
                        if (_adapter.GetType() == typeof(CachedZuneDatabaseReader))
                        {
                            _container.Rebind<IZuneDbAdapter>().To<ZuneDbAdapter>().InSingletonScope();
                            _adapter = _container.Get<IZuneDbAdapter>();

                            InitializeDatabase();
                        }
                        else
                        {
                            return DbLoadResult.Failed;
                        }
                    }
                    else
                    {
                        return DbLoadResult.Success;
                    }
                }
            }
            catch (NotSupportedException e)
            {
                //if the version of the dll is not the version that this software supports
                //we should display an error but still attempt to load the database because it might work
                this.InlineZuneMessage.ShowMessage(ErrorMode.Warning,e.Message);

                return DbLoadResult.Success;
            }
            catch(FileNotFoundException e)
            {
                //if ZuneDBApi.dll cannot be found this will be thrown
                this.InlineZuneMessage.ShowMessage(ErrorMode.Error,e.Message);

                return DbLoadResult.Failed;
            }

            return DbLoadResult.Failed;
        }

        private void ShowSelectAudioFilesViewWithError()
        {
            ShowSelectAudioFilesView();
            this.InlineZuneMessage.ShowMessage(ErrorMode.Error,"Error loading zune database");
        }

        private void ShowSelectAudioFilesView()
        {
            var firstPage = _container.Get<SelectAudioFilesViewModel>();
            firstPage.CanSwitchToNewMode = false;

            this.CurrentPage = firstPage;
        }

        private void ShowAlbumListView()
        {
            ReadDatabase();
            var webAlbumListViewModel = _container.Get<WebAlbumListViewModel>();

            this.CurrentPage = webAlbumListViewModel;
        }

        private void ReadDatabase()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                    //UIDispatcher.GetDispatcher().Invoke(new Action(()=>
                    //{
                    //    foreach (AlbumDetails newAlbum in _adapter.ReadAlbums())
                    //    {
                    //        _model.AlbumsFromDatabase.Add(new AlbumDetailsViewModel(newAlbum));
                    //    }
                    //}));

                    foreach (AlbumDetails newAlbum in _adapter.ReadAlbums())
                    {
                        Thread.Sleep(500);

                        AlbumDetails album = newAlbum;

                        UIDispatcher.GetDispatcher().Invoke(new Action(() => _model.AlbumsFromDatabase.Add(
                            new AlbumDetailsViewModel(album))));
                    }
            });
        }

        public void ShuttingDown()
        {
            var albumListViewModel = _container.Get<WebAlbumListViewModel>();
            WriteCacheToFile(albumListViewModel);
            SaveSettings(albumListViewModel);
        }

        private void SaveSettings(WebAlbumListViewModel albumListViewModel)
        {
            //save which ever view is set to the first one to the settings
            var firstPage = _container.Get<IFirstPage>();

            if (firstPage.GetType() == typeof (WebAlbumListViewModel))
                Settings.Default.FirstViewToLoad = FirstViews.WebAlbumListViewModel;

            if (firstPage.GetType() == typeof (SelectAudioFilesViewModel))
                Settings.Default.FirstViewToLoad = FirstViews.SelectAudioFilesViewModel;

            //remember the sort order so it can be sorted the same way next time
            var sortOrder = albumListViewModel.SortViewModel.SortOrder;
            Settings.Default.SortOrder = sortOrder;
            Settings.Default.Save();
        }

        private void WriteCacheToFile(WebAlbumListViewModel albumListViewModel)
        {
            //TODO: attempt to seriailze data if application was forcibly shut down

            List<AlbumDetails> albums = (from album in albumListViewModel.Albums
                                         select new AlbumDetails
                                                    {
                                                        LinkStatus = album.LinkStatus,
                                                        WebAlbumMetaData = album.WebAlbumMetaData,
                                                        ZuneAlbumMetaData = album.ZuneAlbumMetaData,
                                                    }).ToList();

            if (albums.Count > 0)
            {
                try
                {
                    var xSer = new XmlSerializer(albums.GetType());

                    using (var fs = new FileStream("zunesoccache.xml", FileMode.Create))
                        xSer.Serialize(fs, albums);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
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
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;

                if (_currentPage.GetType() == typeof (SelectAudioFilesViewModel))
                    _container.Rebind<IFirstPage>().To<SelectAudioFilesViewModel>().InSingletonScope();

                if (_currentPage.GetType() == typeof(WebAlbumListViewModel))
                {
                    _container.Rebind<IFirstPage>().To<WebAlbumListViewModel>().InSingletonScope();

                    if (_model.AlbumsFromDatabase.Count == 0)
                    {
                        InitializeDatabase();
                        ReadDatabase();
                    }
                }

                RaisePropertyChanged("CurrentPage");
            }
        }

        private void CheckForUpdates()
        {
            string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                  Settings.Default.UpdateExeName);

            if (Settings.Default.CheckForUpdates)
            {
                try
                {
                    //do update checking stuff here
                    var updateManager = UpdateManager.Instance;

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
                                this.UpdateAvailable = true;
                        }
                        catch { }
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
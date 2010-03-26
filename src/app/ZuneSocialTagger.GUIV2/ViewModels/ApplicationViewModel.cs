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
        private IZuneDbAdapter _adapter;
        private ViewModelBase _currentPage;
        private bool _updateAvailable;

        public ApplicationViewModel(IZuneWizardModel model, IZuneDbAdapter adapter, IKernel container)
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

            InlineZuneMessage = new InlineZuneMessageViewModel();

            if (Settings.Default.FirstViewToLoad == FirstViews.SelectAudioFilesViewModel)
                this.CurrentPage = _container.Get<SelectAudioFilesViewModel>();
            else
                this.CurrentPage = _container.Get<WebAlbumListViewModel>();
        }

        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand ShowAboutSettingsCommand { get; private set; }
        public InlineZuneMessageViewModel InlineZuneMessage { get; set; }

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

                if (_currentPage.GetType() == typeof (SelectAudioFilesViewModel))
                    _container.Rebind<IFirstPage>().To<SelectAudioFilesViewModel>().InSingletonScope();

                //anytime the view gets switch we want to rebind and load the database
                if (_currentPage.GetType() == typeof (WebAlbumListViewModel))
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

        private void DisplayErrorMessage(ErrorMessage message)
        {
            InlineZuneMessage.ShowMessage(message.ErrorMode, message.Message);
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
            CurrentPage = (ViewModelBase) _container.Get(viewType);
        }

        private void SetupCommandBindings()
        {
            ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            UpdateCommand = new RelayCommand(ShowUpdate);
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
                        if (_adapter.GetType() == typeof (CachedZuneDatabaseReader))
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
                InlineZuneMessage.ShowMessage(ErrorMode.Warning, e.Message);

                return DbLoadResult.Success;
            }
            catch (FileNotFoundException e)
            {
                //if ZuneDBApi.dll cannot be found this will be thrown
                InlineZuneMessage.ShowMessage(ErrorMode.Error, e.Message);

                return DbLoadResult.Failed;
            }

            return DbLoadResult.Failed;
        }

        private void ShowSelectAudioFilesViewWithError()
        {
            ShowSelectAudioFilesView();
            InlineZuneMessage.ShowMessage(ErrorMode.Error, "Error loading zune database");
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
            ThreadPool.QueueUserWorkItem(_ =>
                 {
                     //UIDispatcher.GetDispatcher().Invoke(new Action(()=>
                     //{
                     //    foreach (AlbumDetails newAlbum in _adapter.ReadAlbums())
                     //    {
                     //        _model.AlbumsFromDatabase.Add(new AlbumDetailsViewModel(newAlbum));
                     //    }
                     //}));

                     //TODO: remember to remove this in production code!!!
                     foreach (AlbumDetails newAlbum in _adapter.ReadAlbums())
                     {
                         Thread.Sleep(50);

                         AlbumDetails album = newAlbum;

                         UIDispatcher.GetDispatcher().Invoke(
                             new Action(() => _model.AlbumsFromDatabase.Add(
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
            SortOrder sortOrder = albumListViewModel.SortViewModel.SortOrder;
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

        private void CheckForUpdates()
        {
            string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                              Settings.Default.UpdateExeName);

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
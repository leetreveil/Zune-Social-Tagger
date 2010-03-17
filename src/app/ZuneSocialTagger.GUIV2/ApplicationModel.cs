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


namespace ZuneSocialTagger.GUIV2
{
    public class ApplicationModel : ViewModelBase
    {
        private IZuneDbAdapter _adapter;
        private readonly IKernel _container;
        private bool _updateAvailable;
        private ViewModelBase _currentPage;

        public ApplicationModel(IZuneDbAdapter adapter, IKernel container)
        {
            _adapter = adapter;
            _container = container;

            CheckForUpdates();
            SetupCommandBindings();
            InitializeDatabase();

            //register for changes to the current view model so we can switch between views
            Messenger.Default.Register<Type>(this,SetupViewSwitching);
        }

        private void SetupViewSwitching(Type viewType)
        {
            if (viewType == typeof(IFirstPage))
                this.CurrentPage = (ViewModelBase)_container.Get<IFirstPage>();

            if (viewType == typeof(WebAlbumListViewModel))
                this.CurrentPage = _container.Get<WebAlbumListViewModel>();

            if (viewType == typeof(SelectAudioFilesViewModel))
                this.CurrentPage = _container.Get<SelectAudioFilesViewModel>();

            if (viewType == typeof(SearchViewModel))
                this.CurrentPage = _container.Get<SearchViewModel>();

            if (viewType == typeof(SearchResultsViewModel))
                this.CurrentPage = _container.Get<SearchResultsViewModel>();

            if (viewType == typeof(DetailsViewModel))
                this.CurrentPage = _container.Get<DetailsViewModel>();
        }

        private void SetupCommandBindings()
        {
            this.ShowAboutSettingsCommand = new RelayCommand(ShowAboutSettings);
            this.UpdateCommand = new RelayCommand(ShowUpdate);
        }

        public string MessageText { get { return "Haggis"; } }

        public ErrorMode ErrorMode { get { return ErrorMode.Error; } }

        private void InitializeDatabase()
        {
            if (_adapter.CanInitialize)
            {
                //since the adapter is initally set to the cache this should always be true
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
                        //if we are loading the actual database but there is an initalizing error...
                        ZuneMessageBox.Show("Error loading zune database", ErrorMode.Error);
                        ShowSelectAudioFilesViewWithError();
                    }
                }
                else
                {
                    ShowAlbumListView();
                }
            }
            else
            {
                //ZuneMessageBox.Show("ZuneDBApi.dll was not found", ErrorMode.Error);
                ShowSelectAudioFilesViewWithError();
            }
        }

        private void ShowSelectAudioFilesViewWithError()
        {
            _container.Rebind<IFirstPage>().To<SelectAudioFilesViewModel>().InSingletonScope();

            var firstPage = _container.Get<SelectAudioFilesViewModel>();
            firstPage.CanSwitchToNewMode = false;

            this.CurrentPage = firstPage;
        }

        private void ShowAlbumListView()
        {
            _container.Rebind<IFirstPage>().To<WebAlbumListViewModel>().InSingletonScope();

            var webAlbumListViewModel = _container.Get<WebAlbumListViewModel>();

            this.CurrentPage = webAlbumListViewModel;
        }

        public void ShuttingDown()
        {
            //TODO: attempt to seriailze data if application was forcibly shut down

            var albumListViewModel = _container.Get<WebAlbumListViewModel>();

            List<AlbumDetails> albums = (from album in albumListViewModel.Albums
                                         select new AlbumDetails
                                                    {
                                                        LinkStatus = album.LinkStatus,
                                                        WebAlbumMetaData = album.WebAlbumMetaData,
                                                        ZuneAlbumMetaData = album.ZuneAlbumMetaData
                                                    }).ToList();

            try
            {
                var xSer = new XmlSerializer(albums.GetType());

                using (var fs = new FileStream("zunesoccache.xml", FileMode.Create))
                    xSer.Serialize(fs, albums);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            var sortOrder = albumListViewModel.SortViewModel.SortOrder;
            Settings.Default.SortOrder = sortOrder;
            Settings.Default.Save();
        }

        public void ShowUpdate()
        {
            new UpdateView().Show();
        }

        public void CloseApplication()
        {
            //base.Close();
        }

        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand ShowAboutSettingsCommand { get; private set; }

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
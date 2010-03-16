using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Caliburn.PresentationFramework.Screens;
using leetreveil.AutoUpdate.Framework;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;


namespace ZuneSocialTagger.GUIV2
{
    public class ApplicationModel : Screen
    {
        private readonly IZuneWizardModel _model;
        private IZuneDbAdapter _adapter;
        private readonly IUnityContainer _container;
        private readonly IServiceLocator _locator;
        private bool _updateAvailable;

        public ApplicationModel(IServiceLocator locator, IZuneWizardModel model,
                                IZuneDbAdapter adapter,IUnityContainer container)
        {
            _locator = locator;
            _model = model;
            _adapter = adapter;
            _container = container;

            CheckForUpdates();

            this.WasShutdown += ApplicationModel_WasShutdown;

            ZuneDllPreDbLoadCheck();
        }

        private void ZuneDllPreDbLoadCheck()
        {
            if (File.Exists("ZuneDBApi.dll"))
            {
                InitializeDatabase();
                _container.RegisterType<IFirstPage, WebAlbumListViewModel>(new ContainerControlledLifetimeManager());
                _model.CurrentPage = (Screen) _locator.GetInstance<IFirstPage>();
            }
            else
            {
                _container.RegisterType<IFirstPage, SelectAudioFilesViewModel>(new ContainerControlledLifetimeManager());

                var firstPage = (SelectAudioFilesViewModel) _locator.GetInstance<IFirstPage>();
                firstPage.CanSwitchToNewMode = false;

                _model.CurrentPage = firstPage;
            }
        }

        private void InitializeDatabase()
        {
            bool initialized = _adapter.Initialize();

            if (!initialized)
            {
                //fall back to the actual zune database if the cache could not be loaded
                if (_adapter.GetType() == typeof(CachedZuneDatabaseReader))
                {
                    _container.RegisterType<IZuneDbAdapter, ZuneDbAdapter>();

                    _adapter = _locator.GetInstance<IZuneDbAdapter>();
                    InitializeDatabase();
                }
                else
                {
                    ZuneMessageBox.Show("Error loading zune database", ErrorMode.Error);
                }
            }
        }

        void ApplicationModel_WasShutdown(object sender, EventArgs e)
        {
            //TODO: attempt to seriailze data if application was forcibly shut down

            var albumListViewModel = _locator.GetInstance<WebAlbumListViewModel>();
            var albums = albumListViewModel.Albums;

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

        public void ShowUpdateView()
        {
            //TODO: dont like this stuff...
            new UpdateView(new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version)).Show();
        }

        public void CloseApplication()
        {
            base.Close();
        }

        public void ShowAboutSettingsView()
        {
            new AboutView().Show();
        }

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set 
            {
                _updateAvailable = value; 
                NotifyOfPropertyChange(() => UpdateAvailable); 
            }
        }

        public IZuneWizardModel Model
        {
            get { return _model; }
        }

        public Screen CurrentPage
        {
            get { return _model.CurrentPage; }
            set
            {
                _model.CurrentPage = value;
                NotifyOfPropertyChange(() => CurrentPage);
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
                        catch {}
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
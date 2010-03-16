using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Caliburn.PresentationFramework.Screens;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.GUIV2.Models;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.GUIV2.Views;


namespace ZuneSocialTagger.GUIV2
{
    public class ApplicationModel : Screen
    {
        private readonly IZuneWizardModel _model;
        private readonly UnityContainer _container;
        private bool _updateAvailable;

        public ApplicationModel(IZuneWizardModel model, UnityContainer container)
        {
            _model = model;
            _container = container;

            _model.CurrentPage = _container.Resolve<WebAlbumListViewModel>();

            CheckForUpdates();

            this.WasShutdown += ApplicationModel_WasShutdown;
        }

        void ApplicationModel_WasShutdown(object sender, EventArgs e)
        {
            //TODO: attempt to seriailze data if application was forcibly shut down

            var selectAudioFilesViewModel = _container.Resolve<WebAlbumListViewModel>();
            var albums = selectAudioFilesViewModel.Albums;
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

            var sortOrder = selectAudioFilesViewModel.SortViewModel.SortOrder;
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
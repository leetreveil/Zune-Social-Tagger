using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Caliburn.PresentationFramework.Screens;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.GUIV2.Models;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.GUIV2.Views;
using System.Windows;


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

            _model.CurrentPage = _container.Resolve<SelectAudioFilesViewModel>();

            CheckForUpdates();
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
                        catch (Exception){}
                    });
                }
                catch (Exception e)
                {
                    //TODO: log that we could not check for updates
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += App_Startup;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            new ZuneWizardDialog().Show();
            ////path where we want to temporarily store the updater executable must be somewhere that
            ////the user has to have write access (can have any file name)
            //string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "socialtaggerupdater.exe");

            //if (GUIV2.Properties.Settings.Default.CheckForUpdates)
            //{
            //    //do update checking stuff here
            //    UpdateManager.UpdateExePath = updaterPath;
            //    UpdateManager.AppFeedUrl = GUIV2.Properties.Settings.Default.UpdateFeedUrl;
            //    UpdateManager.UpdateExe = GUIV2.Properties.Resources.socialtaggerupdater;
            //    //always clean up at startup because we cant do it at the end
            //    UpdateManager.CleanUp();

            //    Update availUpd;
            //    if (UpdateManager.CheckForUpdate(out availUpd) && availUpd != null)
            //    {
            //        new UpdateView(new UpdateViewModel(availUpd.Version)).Show();
            //    }
            //    else
            //    {
            //        new ZuneWizardDialog().Show();
            //    }

            //}
            //else
            //{
            //    new ZuneWizardDialog().Show();
            //}
        }
    }
}

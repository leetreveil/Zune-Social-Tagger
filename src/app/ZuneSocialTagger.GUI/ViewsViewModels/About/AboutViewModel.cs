using System;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.Properties;

namespace ZuneSocialTagger.GUI.ViewsViewModels.About
{
    public class AboutViewModel
    {
        public AboutViewModel()
        {
            this.OpenWebsiteCommand = new RelayCommand(OpenWebsite);
        }

        public RelayCommand OpenWebsiteCommand { get; private set; }

        public string Version
        {
            get { return String.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version); }
        }

        public string ApplicationUrl
        {
            get { return Settings.Default.AppBaseUrl; }
        }

        public bool UpdateEnabled
        {
            get { return Settings.Default.CheckForUpdates; }
        }

        private static void OpenWebsite()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Settings.Default.AppBaseUrl));
        }
    }
}
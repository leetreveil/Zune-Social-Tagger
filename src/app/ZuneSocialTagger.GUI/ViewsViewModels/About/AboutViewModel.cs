using System;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.Properties;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;

namespace ZuneSocialTagger.GUI.ViewsViewModels.About
{
    public class AboutViewModel
    {
        public AboutViewModel(ApplicationViewModel avm)
        {
            this.OpenWebsiteCommand = new RelayCommand(OpenWebsite);
            this.ReloadDbCommand = new RelayCommand(delegate { avm.ReadActualDatabase(); });
        }

        public RelayCommand OpenWebsiteCommand { get; private set; }
        public RelayCommand ReloadDbCommand { get; private set; }

        public string Version
        {
            get { return String.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version); }
        }

        private static void OpenWebsite()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Settings.Default.AppBaseUrl));
        }
    }
}
using System;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.Properties;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class AboutViewModel
    {
        public AboutViewModel()
        {
            SetCanUpdateCommand = new RelayCommand<bool>(SetCanUpdate);
        }

        public RelayCommand<bool> SetCanUpdateCommand { get; set; }

        public string Version
        {
            get
            {
                return String.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version);
            }
        }

        public void SetCanUpdate(bool update)
        {
            if (update == UpdateEnabled) return;
                Settings.Default.CheckForUpdates = update;
        }

        public bool UpdateEnabled
        {
            get
            {
                return Settings.Default.CheckForUpdates;
            }
        }
    }
}
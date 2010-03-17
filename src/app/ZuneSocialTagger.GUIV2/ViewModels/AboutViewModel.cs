using System;
using System.Reflection;
using GalaSoft.MvvmLight.Command;

namespace ZuneSocialTagger.GUIV2.ViewModels
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

            Properties.Settings.Default.CheckForUpdates = update;
            Properties.Settings.Default.Save();
        }

        public bool UpdateEnabled
        {
            get
            {
                return Properties.Settings.Default.CheckForUpdates;
            }
        }
    }
}
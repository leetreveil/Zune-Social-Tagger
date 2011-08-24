using System;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.Properties;
using System.Windows.Input;

namespace ZuneSocialTagger.GUI.ViewsViewModels.About
{
    public class AboutViewModel
    {
        private ICommand _openWebsiteCommand;
        public ICommand OpenWebsiteCommand 
        {
            get
            {
                if (_openWebsiteCommand == null)
                    _openWebsiteCommand = new RelayCommand(OpenWebsite);

                return _openWebsiteCommand;
            }
        }

        private ICommand _openTwitterCommand;
        public ICommand OpenTwitterCommand
        {
            get
            {
                if (_openTwitterCommand == null)
                    _openTwitterCommand = new RelayCommand(OpenTwitter);

                return _openTwitterCommand;
            }
        }

        public string Version
        {
            get { return String.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version); }
        }

        private void OpenWebsite()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Settings.Default.AppBaseUrl));
        }

        private void OpenTwitter()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("http://twitter.com/#!/leetreveil"));
        }
    }
}
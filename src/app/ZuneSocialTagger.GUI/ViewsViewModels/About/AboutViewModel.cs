using System;
using System.Reflection;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUI.Properties;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using System.Windows.Input;

namespace ZuneSocialTagger.GUI.ViewsViewModels.About
{
    public class AboutViewModel
    {
        private ApplicationViewModel _avm;

        public AboutViewModel(ApplicationViewModel avm)
        {
            _avm = avm;
        }

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

        private ICommand _reloadDbCommand;
        public ICommand ReloadDbCommand
        {
            get
            {
                if (_reloadDbCommand == null)
                    _reloadDbCommand = new RelayCommand(_avm.ReadActualDatabase);

                return _reloadDbCommand;
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
    }
}
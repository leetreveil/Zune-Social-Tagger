using System;
using System.Windows.Input;
using ZuneSocialTagger.GUIV2.Commands;
using leetreveil.AutoUpdate.Framework;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class UpdateViewModel : ViewModelBase
    {
        private readonly Version _versionAvailable;
        private RelayCommand _applyUpdateCommand;

        public UpdateViewModel(Version versionAvailable)
        {
            _versionAvailable = versionAvailable;
        }

        public string Version { get { return _versionAvailable.ToString(); } }


        public ICommand ApplyUpdateCommand
        {
            get
            {
                if (_applyUpdateCommand == null)
                    _applyUpdateCommand = new RelayCommand(UpdateManager.ApplyUpdate);

                return _applyUpdateCommand;
            }
        }
    }
}
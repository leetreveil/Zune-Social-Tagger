using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Update
{
    public class UpdateViewModel
    {
        private readonly Version _versionAvailable;

        public UpdateViewModel(Version versionAvailable)
        {
            _versionAvailable = versionAvailable;
        }

        private ICommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                    _updateCommand = new RelayCommand(ApplyUpdate);

                return _updateCommand;
            }
        }

        public string Version
        {
            get { return _versionAvailable.ToString(); }
        }

        public void ApplyUpdate()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/leetreveil/Zune-Social-Tagger/downloads"));
        }
    }
}
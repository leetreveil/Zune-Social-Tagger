using System;
using System.Windows.Input;
using Caliburn.PresentationFramework.Screens;
using leetreveil.AutoUpdate.Framework;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class UpdateViewModel : Screen
    {
        private readonly Version _versionAvailable;

        public UpdateViewModel(Version versionAvailable)
        {
            _versionAvailable = versionAvailable;
        }

        public string Version { get { return _versionAvailable.ToString(); } }

        public void ApplyUpdate()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                UpdateManager.Instance.ApplyUpdate();
            }
            catch (Exception)
            {
                //TODO: log error that the update could not be applied
                Mouse.OverrideCursor = null;
            }
        }
    }


}
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using leetreveil.AutoUpdate.Framework;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for ZuneWizardDialog.xaml
    /// </summary>
    public partial class ZuneWizardDialog : DraggableWindow
    {
        private readonly ZuneWizardViewModel _zuneWizardViewModel;

        public ZuneWizardDialog()
        {
            InitializeComponent();

            _zuneWizardViewModel = new ZuneWizardViewModel();
            this.DataContext = _zuneWizardViewModel;
            this.Loaded += ZuneWizardDialog_Loaded;
        }

        void ZuneWizardDialog_Loaded(object sender, RoutedEventArgs e)
        {
            //path where we want to temporarily store the updater executable must be somewhere that
            //the user has to have write access (can have any file name)
            string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"socialtaggerupdater.exe");

            if (Properties.Settings.Default.CheckForUpdates)
            {
                //do update checking stuff here
                UpdateManager.UpdateExePath = updaterPath;
                UpdateManager.AppFeedUrl = "zunesocupdatefeedx.xml";
                UpdateManager.UpdateExe = Properties.Resources.socialtaggerupdater;
                //always clean up at the beginning of the exe because we cant do it at the end
                UpdateManager.CleanUp();
                //if an update is available then show the update window
                UpdateManager.CheckForUpdate(update => Dispatcher.Invoke(new Action(() => new UpdateView(
                                                                                 new UpdateViewModel(update.Version)).Show())));
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}

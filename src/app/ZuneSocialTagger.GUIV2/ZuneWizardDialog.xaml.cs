using System;
using System.IO;
using System.Threading;
using System.Windows;
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

        private void ZuneWizardDialog_Loaded(object sender, RoutedEventArgs e)
        {
            //string updaterPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            //                                  Properties.Settings.Default.UpdateExeName);

            //if (Properties.Settings.Default.CheckForUpdates)
            //{
            //    //do update checking stuff here
            //    UpdateManager.UpdateExePath = updaterPath;
            //    UpdateManager.AppFeedUrl = Properties.Settings.Default.UpdateFeedUrl;
            //    UpdateManager.UpdateExe = Properties.Resources.socialtaggerupdater;
            //    //always clean up at startup because we cant do it at the end
            //    UpdateManager.CleanUp();

            //    Update availUpd;

            //    ThreadPool.QueueUserWorkItem(state =>
            //         {
            //             if (UpdateManager.CheckForUpdate(out availUpd) && availUpd != null)
            //             {
            //                 Dispatcher.Invoke(new Action(() =>
            //                      {

            //                          var updView = new UpdateView(new UpdateViewModel(availUpd.Version),this);
            //                          updView.Top = this.Top;
            //                          updView.Left = this.Left;
            //                          updView.Show();

            //                          this.Hide();
            //                      }));
            //             }
            //         });

            //}
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void AboutSettings_Click(object sender, RoutedEventArgs e)
        {
            var view = new AboutView {ShowInTaskbar = false};
            view.Show();
        }
    }
}
using System;
using System.IO;
using System.Windows.Threading;
using Ninject;
using Utilities;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.ViewModels;
using ZuneSocialTagger.GUI.Views;
using GalaSoft.MvvmLight.Threading;

namespace ZuneSocialTagger.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly StandardKernel Container = new StandardKernel();
        private static readonly ExceptionLogger LoggerForStrings = new ExceptionLogger();
        private static readonly StringLogger StringLogger = new StringLogger();
        private static readonly ExceptionLogger LoggerForEmail = new ExceptionLogger();

        public App()
        {
            SetupUnhandledExceptionLogging();

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            DispatcherHelper.Initialize();

            string pathToZuneSocAppDataFolder = Path.Combine(Environment.GetFolderPath(
                                                             Environment.SpecialFolder.ApplicationData), "Zune Social Tagger");

            if (!Directory.Exists(pathToZuneSocAppDataFolder))
                Directory.CreateDirectory(pathToZuneSocAppDataFolder);

            Settings.Default.AppDataFolder = pathToZuneSocAppDataFolder;

            SetupBindings();

            var appViewModel = Container.Get<ApplicationViewModel>();
            var appView = new ApplicationView {DataContext = appViewModel};

            appView.Loaded += delegate { appViewModel.ApplicationViewHasLoaded(); };

            appView.Show();
        }

        private void SetupUnhandledExceptionLogging()
        {
            var emailLogger = new EmailLogger
                                  { 
                                      EmailFrom = "error@zunesocialtagger.net", 
                                      EmailServer = "smtp.ntlworld.com", 
                                      EmailTo = "leetreveil@gmail.com" 
                                  };

            LoggerForStrings.AddLogger(StringLogger);
            LoggerForEmail.AddLogger(emailLogger);
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LoggerForStrings.LogException(e.Exception);

            ErrorReportDialog.Show(StringLogger.ErrorLog,() => 
            {
                try
                {
                    LoggerForEmail.LogException(e.Exception);
                }
                catch{}
                finally 
                {
                    Current.Shutdown();
                }


            }, () => Current.Shutdown());

            e.Handled = true;
        }

        private void SetupBindings()
        {
            Container.Bind<Dispatcher>().ToMethod(context => this.Dispatcher);
            Container.Bind<ZuneWizardModel>().ToSelf().InSingletonScope();
            Container.Bind<IZuneDatabaseReader>().To<ZuneDatabaseReader>().InSingletonScope();

            Container.Bind<ApplicationViewModel>().ToSelf().InSingletonScope();
            Container.Bind<CachedZuneDatabaseReader>().ToSelf().InSingletonScope();
            Container.Bind<WebAlbumListViewModel>().ToSelf().InSingletonScope();
        }
    }

    public class StringLogger : LoggerImplementation
    {
        public string ErrorLog { get; private set; }

        public override void LogError(string error)
        {
            ErrorLog = error;
        }
    }

}

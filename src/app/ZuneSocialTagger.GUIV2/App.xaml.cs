using System;
using System.Diagnostics;
using System.IO;
using Ninject;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.GUIV2.Views;


namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly StandardKernel Container = new StandardKernel();

        public App()
        {
            UIDispatcher.SetDispatcher(this.Dispatcher);

            string pathToZuneSocAppDataFolder = Path.Combine(Environment.GetFolderPath(
                                                             Environment.SpecialFolder.ApplicationData), "Zune Social Tagger");

            if (!Directory.Exists(pathToZuneSocAppDataFolder))
                Directory.CreateDirectory(pathToZuneSocAppDataFolder);

            Settings.Default.AppDataFolder = pathToZuneSocAppDataFolder;

            SetupBindings();

            new ApplicationView(Container.Get<ApplicationViewModel>()).Show();
        }

        private void SetupBindings()
        {
            Container.Bind<IZuneWizardModel>().To<ZuneWizardModel>().InSingletonScope();
            Container.Bind<IZuneDatabaseReader>().To<TestZuneDatabaseReader>().InSingletonScope();

            Container.Bind<ApplicationViewModel>().ToSelf().InSingletonScope();
            Container.Bind<CachedZuneDatabaseReader>().ToSelf().InSingletonScope();
        }
    }

}

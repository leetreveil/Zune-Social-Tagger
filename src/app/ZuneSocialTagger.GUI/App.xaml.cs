using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Ninject;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.ViewModels;
using ZuneSocialTagger.GUI.Views;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;
using Album = ZuneSocialTagger.Core.ZuneWebsite.Album;

namespace ZuneSocialTagger.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly StandardKernel Container = new StandardKernel();

        public App()
        {
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

        private void SetupBindings()
        {
            Container.Bind<Dispatcher>().ToMethod(context => this.Dispatcher);
            Container.Bind<ZuneWizardModel>().ToSelf().InSingletonScope();
            Container.Bind<IZuneDatabaseReader>().To<TestZuneDatabaseReader>().InSingletonScope();

            Container.Bind<ApplicationViewModel>().ToSelf().InSingletonScope();
            Container.Bind<CachedZuneDatabaseReader>().ToSelf().InSingletonScope();
        }
    }

}

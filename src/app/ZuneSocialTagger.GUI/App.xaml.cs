﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Ninject;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Controls;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Details;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;

namespace ZuneSocialTagger.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            WpfSingleInstanceByEventWaitHandle.WpfSingleInstance.Make();
            this.Startup += App_Startup;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            DispatcherHelper.Initialize();

            Settings.Default.AppDataFolder = GetUserDataPath();

            var container = new StandardKernel();
            SetupBindings(container);

            var appView = container.Get<ApplicationView>();
            appView.Show();

            var appViewModel = container.Get<ApplicationViewModel>();
            appView.DataContext = appViewModel;
            Current.Exit += delegate { appViewModel.ApplicationIsShuttingDown(); };

            appViewModel.ViewHasLoaded();
        }

        private static void SetupBindings(StandardKernel container)
        {
#if DEBUG
            container.Bind<IZuneDatabaseReader>().To<TestZuneDatabaseReader>().InSingletonScope();
#else
            container.Bind<IZuneDatabaseReader>().To<ZuneDatabaseReader>().InSingletonScope();
#endif
            //Container.Bind<IApplicationViewModel>().To<ApplicationViewModel>();
            container.Bind<ViewLocator>().ToSelf().InSingletonScope();

            //we need the web view model to be a singleton because we want to be able to continue
            //downloading data while linking etc
            container.Bind<SharedModel>().ToSelf().InSingletonScope();
            container.Bind<SelectAudioFilesViewModel>().ToSelf().InSingletonScope();
            container.Bind<WebAlbumListViewModel>().ToSelf().InSingletonScope();
            container.Bind<SearchViewModel>().ToSelf().InSingletonScope();
            container.Bind<DetailsViewModel>().ToSelf().InSingletonScope();
            container.Bind<SafeObservableCollection<AlbumDetailsViewModel>>().ToSelf().InSingletonScope();
            container.Bind<ApplicationViewModel>().ToSelf().InSingletonScope();

            //set some views to remember their state
            container.Bind<WebAlbumListView>().ToSelf().InSingletonScope();
        }

        private static string GetUserDataPath()
        {
            string pathToZuneSocAppDataFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "Zune Social Tagger");

            if (!Directory.Exists(pathToZuneSocAppDataFolder))
                Directory.CreateDirectory(pathToZuneSocAppDataFolder);

            return pathToZuneSocAppDataFolder;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            DisplayException(e.Exception);
            e.Handled = true;
        }

        public static void DisplayException(Exception ex) 
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => 
            {
                ErrorReportDialog.Show(ExceptionLogger.LogException(ex), null);
            });
        }
    }
}

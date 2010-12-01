﻿using System;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Collections.Generic;
using System.Windows;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;

namespace ZuneSocialTagger.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly StandardKernel Container = new StandardKernel();
        //private static readonly ExceptionLogger LoggerForStrings = new ExceptionLogger();
        //private static readonly StringLogger StringLogger = new StringLogger();

        public App()
        {
            this.Startup += App_Startup;
        }

        void App_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            //SetupUnhandledExceptionLogging();

           // this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            DispatcherHelper.Initialize();

            Settings.Default.AppDataFolder = GetUserDataPath();

            SetupBindings();

            var appView = new ApplicationView();
            appView.Show();

            var appViewModel = Container.Get<ApplicationViewModel>();
            appView.DataContext = appViewModel;
            appViewModel.ViewHasLoaded();
        }

        private static void SetupBindings()
        {
#if DEBUG
            Container.Bind<IZuneDatabaseReader>().To<TestZuneDatabaseReader>().InSingletonScope();
#else
            Container.Bind<IZuneDatabaseReader>().To<ZuneDatabaseReader>().InSingletonScope();
#endif
            //Container.Bind<IApplicationViewModel>().To<ApplicationViewModel>();
            Container.Bind<IViewLocator>().To<ViewLocator>().InSingletonScope();

            //songs the user loads from file are stored here
            Container.Bind<IZuneAudioFileRetriever>().To<ZuneAudioFileRetriever>().InSingletonScope();

            //we need the web view model to be a singleton because we want to be able to continue
            //downloading data while linking etc
            Container.Bind<SharedModel>().ToSelf().InSingletonScope();
            Container.Bind<SelectAudioFilesViewModel>().ToSelf().InSingletonScope();
            Container.Bind<WebAlbumListViewModel>().ToSelf().InSingletonScope();
            Container.Bind<SearchViewModel>().ToSelf().InSingletonScope();
            Container.Bind<DetailsViewModel>().ToSelf().InSingletonScope();
            Container.Bind<SafeObservableCollection<AlbumDetailsViewModel>>().ToSelf().InSingletonScope();
            Container.Bind<ApplicationViewModel>().ToSelf().InSingletonScope();

            //set some views to remember their state
            Container.Bind<WebAlbumListView>().ToSelf().InSingletonScope();
        }

        private static void SetupUnhandledExceptionLogging()
        {
            //LoggerForStrings.AddLogger(StringLogger);
        }

        private static string GetUserDataPath()
        {
            string pathToZuneSocAppDataFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "Zune Social Tagger");

            if (!Directory.Exists(pathToZuneSocAppDataFolder))
                Directory.CreateDirectory(pathToZuneSocAppDataFolder);

            return pathToZuneSocAppDataFolder;
        }

        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //LoggerForStrings.LogException(e.Exception);
            //ErrorReportDialog.Show(StringLogger.ErrorLog,null);
            e.Handled = true;
        }
    }

    //public class StringLogger : LoggerImplementation
    //{
    //    public string ErrorLog { get; private set; }

    //    public override void LogError(string error)
    //    {
    //        ErrorLog = error;
    //    }
    //}

}

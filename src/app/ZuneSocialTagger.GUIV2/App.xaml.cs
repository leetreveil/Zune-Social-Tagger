using System;
using System.Diagnostics;
using System.IO;
using ZuneSocialTagger.GUIV2.Properties;
using ZuneSocialTagger.GUIV2.ViewModels;


namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            new ViewModelLocator();
            UIDispatcher.SetDispatcher(this.Dispatcher);

            string pathToZuneSocAppDataFolder = Path.Combine(Environment.GetFolderPath(
                                                             Environment.SpecialFolder.ApplicationData), "Zune Social Tagger");

            if (!Directory.Exists(pathToZuneSocAppDataFolder))
                Directory.CreateDirectory(pathToZuneSocAppDataFolder);

            Settings.Default.AppDataFolder = pathToZuneSocAppDataFolder;

            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        //void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    Debug.WriteLine("something reaaaaaaaaaaaaaaly bad happened");
        //}
    }

}

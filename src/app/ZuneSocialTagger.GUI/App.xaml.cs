using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ZuneSocialTagger.GUI.Views;

namespace ZuneSocialTagger.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Create the ViewModel and expose it using the View's DataContext
            var view = new MainView();
            var detailsView = new DetailsView();
            var detailsViewModel = new ViewModels.DetailsViewModel();

            view.DataContext = new ViewModels.MainViewModel();
            detailsView.DataContext = detailsViewModel;


            detailsViewModel.SetupWebsiteAlbumMetaDataViewModelDefaults();


            detailsView.Show();
        }
    }
}

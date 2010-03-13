using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.ViewModels;
using Caliburn.PresentationFramework.ApplicationModel;


namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : CaliburnApplication
    {
        private readonly UnityContainer _container;

        public App()
        {
            _container = new UnityContainer();
            _container.RegisterType<IZuneWizardModel, ZuneWizardModel>(new ContainerControlledLifetimeManager());


            //setting the SelectAutoFilesViewModel to be a singleton, 
            //the database wont be loaded each time the viewmodel is constructed now
            _container.RegisterType<WebAlbumListViewModel, WebAlbumListViewModel>(
                new ContainerControlledLifetimeManager());

            _container.RegisterType<SearchHeaderViewModel, SearchHeaderViewModel>(
                new ContainerControlledLifetimeManager());

            _container.RegisterType<ExpandedAlbumDetailsViewModel, ExpandedAlbumDetailsViewModel>(
                new ContainerControlledLifetimeManager());
            
            _container.RegisterType<IZuneDatabaseReader, TestZuneDatabaseReader>();
            _container.RegisterInstance(_container);
        }

        protected override object CreateRootModel()
        {
            return _container.Resolve<ApplicationModel>();
        }
    }
}

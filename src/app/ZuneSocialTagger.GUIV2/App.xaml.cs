using Caliburn.Core.Configuration;
using Caliburn.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.ViewModels;
using Caliburn.PresentationFramework.ApplicationModel;
using ZuneSocialTagger.ZunePlugin;


namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : CaliburnApplication
    {
        protected override object CreateRootModel()
        {
            return Container.GetInstance<ApplicationModel>();
        }

        protected override IServiceLocator CreateContainer()
        {
            var container = new UnityContainer();
            var adapter = new UnityAdapter(container);

            container.RegisterType<IZuneWizardModel, ZuneWizardModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IZuneDatabaseReader, ZuneDatabaseReader>(new ContainerControlledLifetimeManager());
            container.RegisterType<IZuneDbAdapter, CachedZuneDatabaseReader>(new ContainerControlledLifetimeManager());

            ////setting the SelectAutoFilesViewModel to be a singleton, 
            ////the database wont be loaded each time the viewmodel is constructed now
            //container.RegisterType<IFirstPage, WebAlbumListViewModel>(
            //    new ContainerControlledLifetimeManager());

            container.RegisterInstance(container);

            return adapter;
        }
    }
}

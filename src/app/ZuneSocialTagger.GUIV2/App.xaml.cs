using Caliburn.PresentationFramework.ApplicationModel;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;

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

            //we will switch out the DatabaseReader to the proper one nearer to release, maybe using #IF Debug
            _container.RegisterType<IZuneDatabaseReader, TestZuneDatabaseReader>();
            _container.RegisterInstance(_container);
        }

        protected override object CreateRootModel()
        {
            return _container.Resolve<ApplicationModel>();
        }
    }
}

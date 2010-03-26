using GalaSoft.MvvmLight;
using leetreveil.AutoUpdate.Framework;
using Ninject;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    /// <summary>
    /// Handles the creation and lifetime of view models, views should databind to properties set here
    /// </summary>
    public class ViewModelLocator
    {
        private static readonly StandardKernel Container = new StandardKernel();

        static ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                //bind to design database
            }
            else
            {
                Container.Bind<IZuneWizardModel>().To<ZuneWizardModel>().InSingletonScope();
                Container.Bind<IZuneDatabaseReader>().To<TestZuneDatabaseReader>().InSingletonScope();
                Container.Bind<IZuneDbAdapter>().To<CachedZuneDatabaseReader>().InSingletonScope();
            }

            Container.Bind<ApplicationModel>().ToSelf().InSingletonScope();

            //by default we register WebAlbumListViewModel as the default view but this can be changed in the future
            Container.Bind<IFirstPage>().To<WebAlbumListViewModel>().InSingletonScope();
            Container.Bind<SearchHeaderViewModel>().ToSelf().InSingletonScope();
        }

        public ApplicationModel Application
        {
            get { return Container.Get<ApplicationModel>(); }
        }

        public UpdateViewModel Update
        {
            get { return new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version); }
        }

        public AboutViewModel About
        {
            get { return new AboutViewModel(); }
        }

        public SuccessViewModel Success
        {
            get { return null; }
        }
    }
}
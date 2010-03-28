using leetreveil.AutoUpdate.Framework;
using Ninject;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.ZunePlugin;

namespace ZuneSocialTagger.GUIV2
{
    /// <summary>
    /// Handles the creation and lifetime of view models, views should databind to properties set here
    /// </summary>
    public class ViewModelLocator
    {
        private static readonly StandardKernel Container = new StandardKernel();

        static ViewModelLocator()
        {
            Container.Bind<IZuneWizardModel>().To<ZuneWizardModel>().InSingletonScope();
            Container.Bind<IZuneDatabaseReader>().To<ZuneDatabaseReader>().InSingletonScope();
            Container.Bind<IZuneDbAdapter>().To<CachedZuneDatabaseReader>().InSingletonScope();
            Container.Bind<ApplicationViewModel>().ToSelf().InSingletonScope();
            Container.Bind<SearchHeaderViewModel>().ToSelf().InSingletonScope();
            Container.Bind<InlineZuneMessageViewModel>().ToSelf().InSingletonScope();
            Container.Bind<IFirstPage>().To<WebAlbumListViewModel>().InSingletonScope();
        }

        public ApplicationViewModel Application
        {
            get { return Container.Get<ApplicationViewModel>(); }
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

        public InlineZuneMessageViewModel InlineZuneMessageView
        {
            get { return Container.Get<InlineZuneMessageViewModel>(); }
        }
    }
}
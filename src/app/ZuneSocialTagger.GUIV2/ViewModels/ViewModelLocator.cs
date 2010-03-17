using System;
using GalaSoft.MvvmLight;
using leetreveil.AutoUpdate.Framework;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    /// <summary>
    /// Handles the creation and lifetime of view models, views should databind to properties set here
    /// </summary>
    public class ViewModelLocator
    {
        static readonly UnityContainer Container = new UnityContainer();

        static ViewModelLocator()
        {
            Container.RegisterInstance(Container);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                //bind to design database
            }
            else
            {
                Container.RegisterType<IZuneWizardModel, ZuneWizardModel>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IZuneDatabaseReader, TestZuneDatabaseReader>(new ContainerControlledLifetimeManager());
                Container.RegisterType<IZuneDbAdapter, CachedZuneDatabaseReader>(new ContainerControlledLifetimeManager());
            }

            Container.RegisterType<ApplicationModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<WebAlbumListViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SearchHeaderViewModel>(new ContainerControlledLifetimeManager());
        }

        public ApplicationModel Application
        {
            get
            {
                try
                {
                    return Container.Resolve<ApplicationModel>();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public SelectAudioFilesViewModel SelectAudioFiles
        {
            get
            {
                return Container.Resolve<SelectAudioFilesViewModel>();
            }
        }

        public WebAlbumListViewModel WebAlbumListView
        {
            get
            {
                return Container.Resolve<WebAlbumListViewModel>();
            }
        }

        public SearchViewModel SearchView
        {
            get
            {
                return Container.Resolve<SearchViewModel>();
            }
        }

        public UpdateViewModel Update
        {
            get
            {
                return new UpdateViewModel(UpdateManager.Instance.NewUpdate.Version);
            }
        }

        public AboutViewModel About
        {
            get
            {
                return new AboutViewModel();
            }
        }
        public SuccessViewModel Success
        {
            get
            {
                return null;
            }
        }
    }
}
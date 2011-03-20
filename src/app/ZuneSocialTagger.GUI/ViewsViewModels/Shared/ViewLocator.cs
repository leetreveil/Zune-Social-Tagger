using System;
using System.Windows.Controls;
using Ninject;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;
using GalaSoft.MvvmLight.Threading;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    /// <summary>
    /// Handles switching between views
    /// </summary>
    public class ViewLocator
    {
        private readonly IKernel _container;
        private UserControl _firstView;

        public event Action<UserControl,ViewModelBase> SwitchToViewRequested = delegate { };

        public ViewLocator(IKernel container)
        {
            _container = container;
        }

        public object SwitchToFirstView()
        {
            ViewModelBase viewModel = null;
            if (_firstView is WebAlbumListView)
            {
                viewModel = Resolve<WebAlbumListViewModel>();
            }
            if (_firstView is SelectAudioFilesView)
            {
                viewModel = Resolve<SelectAudioFilesViewModel>();
            }
            SwitchToViewRequested.Invoke(_firstView, viewModel);
            return viewModel;
        }

        public TViewModel SwitchToView<TView, TViewModel>() where TView : UserControl where TViewModel : ViewModelBase
        {
            TView viewToSwitchTo;
            TViewModel viewModel = Resolve<TViewModel>();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                viewToSwitchTo = Resolve<TView>();

                if (viewToSwitchTo is WebAlbumListView || viewToSwitchTo is SelectAudioFilesView)
                    _firstView = viewToSwitchTo;

                 viewToSwitchTo.DataContext = viewModel;
                 SwitchToViewRequested.Invoke(viewToSwitchTo, viewModel);
                 
            });

            return viewModel;
        }

        public T Resolve<T>()
        {
            return _container.Get<T>();
        }
    }
}
using System;
using System.Windows.Controls;
using Ninject;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.GUI.ViewsViewModels;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;


namespace ZuneSocialTagger.GUI.Shared
{
    /// <summary>
    /// Handles switching between views
    /// </summary>
    public class ViewLocator
    {
        private readonly IKernel _container;
        private UserControl _firstView;

        public ViewModelBase CurrentViewModel;

        public ViewLocator(IKernel container)
        {
            _container = container;
        }

        public UserControl SwitchToFirstView()
        {
            Messenger.Default.Send<UserControl>(_firstView);
            return (UserControl) _container.Get(_firstView.GetType());
        }

        public TViewModel SwitchToView<TView, TViewModel>() where TView : UserControl where TViewModel : ViewModelBase
        {
            TView viewToSwitchTo;
            TViewModel viewModel = _container.Get<TViewModel>();

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                viewToSwitchTo = _container.Get<TView>();

                if (viewToSwitchTo is WebAlbumListView || viewToSwitchTo is SelectAudioFilesView)
                    _firstView = viewToSwitchTo;

                viewToSwitchTo.DataContext = viewModel;

                Messenger.Default.Send<UserControl>(viewToSwitchTo);
            });

            CurrentViewModel = viewModel;

            return viewModel;
        }
    }
}
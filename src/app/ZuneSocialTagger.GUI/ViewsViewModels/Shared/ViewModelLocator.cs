using System;
using Ninject;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;
using ZuneSocialTagger.GUI.ViewsViewModels.SelectAudioFiles;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    /// <summary>
    /// Handles switching between viewmodels, also remembers when the first view model has been switched
    /// </summary>
    public class ViewModelLocator : IViewModelLocator
    {
        private readonly IKernel _container;

        private ViewModelBase _firstViewModel { get; set; }
        public event Action<ViewModelBase> SwitchToViewModelRequested = delegate { };

        public ViewModelLocator(IKernel container)
        {
            _container = container;
        }

        public void SwitchToFirstViewModel()
        {
            if (_firstViewModel.GetType() == typeof(WebAlbumListViewModel))
            {
                _firstViewModel = _container.Get<WebAlbumListViewModel>();
            }
            else
            {
                _firstViewModel = _container.Get<SelectAudioFilesViewModel>();
            }

            SwitchToViewModelRequested.Invoke(_firstViewModel);
        }

        public T SwitchToViewModel<T>()
        {
            T viewModelToSwitchTo = Resolve<T>();

            if (typeof(T) == typeof(WebAlbumListViewModel) || typeof(T) == typeof(SelectAudioFilesViewModel))
            {
                _firstViewModel = viewModelToSwitchTo as ViewModelBase;
            }

            SwitchToViewModelRequested.Invoke(viewModelToSwitchTo as ViewModelBase);
            return viewModelToSwitchTo;
        }

        public T Resolve<T>()
        {
            return _container.Get<T>();
        }
    }
}
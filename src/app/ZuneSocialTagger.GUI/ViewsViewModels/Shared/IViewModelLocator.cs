using System;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    public interface IViewModelLocator
    {
        event Action<ViewModelBase> SwitchToViewModelRequested;
        T SwitchToViewModel<T>();
        T Resolve<T>();
        void SwitchToFirstViewModel();
    }
}
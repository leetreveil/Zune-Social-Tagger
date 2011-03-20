using System;
using System.Windows.Controls;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    public interface IViewLocator
    {
        event Action<UserControl, ViewModelBase> SwitchToViewRequested;
        T2 SwitchToView<T, T2>() where T : UserControl where T2 : ViewModelBase;
        T Resolve<T>();
        object SwitchToFirstView();
    }
}
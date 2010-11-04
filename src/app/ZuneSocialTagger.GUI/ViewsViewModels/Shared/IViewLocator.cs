using System;
using System.Windows.Controls;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    public interface IViewLocator
    {
        event Action<UserControl> SwitchToViewRequested;
        T2 SwitchToView<T, T2>() where T : UserControl;
        T Resolve<T>();
        void SwitchToFirstView();
    }
}
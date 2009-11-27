using System;
using System.Windows.Threading;
namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public abstract class ViewModelBase : NotifyPropertyChangedImpl
    {
        protected ViewModelBase()
        {
            this.UIDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected Dispatcher UIDispatcher { get; private set; }
    }
}
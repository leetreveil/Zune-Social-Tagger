using System;
using System.Threading;
using System.Windows.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public abstract class ZuneWizardPageViewModelBase : ViewModelBase
    {
        bool _isCurrentPage;

        public event EventHandler MoveNextOverride;
        public event EventHandler MoveNextClicked;

        public void InvokeMoveNextClicked()
        {
            EventHandler handler = MoveNextClicked;
            if (handler != null) handler(this, new EventArgs());
        }

        protected void OnMoveNextOverride()
        {
            EventHandler mNextoverride = MoveNextOverride;
            if (mNextoverride != null) mNextoverride(this, new EventArgs());
        }

        internal virtual string NextButtonText
        {
            get { return "Next"; }
        }

        internal virtual string BackButtonText
        {
            get { return "Back"; }
        }

        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set
            {
                if (value != _isCurrentPage)
                {
                    _isCurrentPage = value;
                    base.InvokePropertyChanged("IsCurrentPage");
                }
            }
        }

        /// <summary>
        /// Returns true if the user can move next, is different to IsValid because this 
        /// is just called when it trys to move next and does not disable the next button
        /// </summary>
        /// <returns></returns>
        internal abstract bool IsNextEnabled();

        internal virtual bool IsNextVisible()
        {
            return true;
        }

        internal virtual bool IsBackVisible()
        {
            return true;
        }
    }
}
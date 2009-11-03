using System.ComponentModel;
using System;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public abstract class ZuneWizardPageViewModelBase : INotifyPropertyChanged
    {
        bool _isCurrentPage;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler MoveNextOverride;


        protected void OnMoveNextOverride()
        {
            EventHandler mNextoverride = MoveNextOverride;
            if (mNextoverride != null) mNextoverride(this, new EventArgs());
        }


        public virtual string NextButtonText
        {
            get { return "Next"; }
        }

        public virtual string BackButtonText
        {
            get { return "Back"; }
        }

        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set
            {
                if (value == _isCurrentPage)
                    return;

                _isCurrentPage = value;
                this.OnPropertyChanged("IsCurrentPage");
            }
        }

        /// <summary>
        /// Returns true if the user has filled in this page properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        internal abstract bool IsValid();

        /// <summary>
        /// Returns true if the user can move next, is different to IsValid because this 
        /// is just called on when it trys to move next and does not disable the next button
        /// </summary>
        /// <returns></returns>
        internal abstract bool CanMoveNext();

        internal virtual bool FlagCanMoveNext { get; set; }

        public virtual void BeforeMoveNext()
        {
            
        }

        public event EventHandler IsMovingNext;

        public void InvokeIsMovingNext()
        {
            EventHandler handler = IsMovingNext;
            if (handler != null) handler(this, new EventArgs());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class WizardPage : UserControl, INotifyPropertyChanged
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
                   this.InvokePropertyChanged("IsCurrentPage");
                }
            }
        }

        /// <summary>
        /// Returns true if the user can move next, is different to IsValid because this 
        /// is just called when it trys to move next and does not disable the next button
        /// </summary>
        /// <returns></returns>
        internal virtual bool IsNextEnabled()
        {
            return true;
        }

        internal virtual bool IsNextVisible()
        {
            return true;
        }

        internal virtual bool IsBackVisible()
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
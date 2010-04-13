using System;
using System.Timers;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class InlineZuneMessageViewModel : NotifyPropertyChangedImpl
    {
        private readonly Timer _timer;
        private string _messageText;
        private ErrorMode _errorMode;

        public event Action ShowMessages = delegate { };
        public event Action HideMessages = delegate { };

        public InlineZuneMessageViewModel()
        {
            _timer = new Timer();
            _timer.Elapsed += delegate { this.HideMessages.Invoke(); };
        }

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                NotifyOfPropertyChange(() => this.MessageText);
            }
        }

        public ErrorMode ErrorMode
        {
            get { return _errorMode; }
            set
            {
                _errorMode = value;
                NotifyOfPropertyChange(() => this.ErrorMode);
            }
        }

        public void ShowMessage(ErrorMode errorMode, string message)
        {
            this.ErrorMode = errorMode;
            this.MessageText = message;
            this.ShowMessages.Invoke();

            ////reset the interval each time a message is displayed
            _timer.Interval = 20000;
            _timer.Start();
        }
    }
}
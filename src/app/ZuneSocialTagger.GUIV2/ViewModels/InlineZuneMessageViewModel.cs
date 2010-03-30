using System.Timers;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class InlineZuneMessageViewModel : NotifyPropertyChangedImpl
    {
        //private readonly Timer _timer;
        private string _messageText;
        private ErrorMode _errorMode;
        //private bool _shouldDisplayMessage;

        //public InlineZuneMessageViewModel()
        //{
        //    _timer = new Timer();
        //    _timer.Elapsed += delegate
        //                          {
        //                              this.ShouldDisplayMessage = false;
        //                          };
        //}

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

        //public bool ShouldDisplayMessage
        //{
        //    get { return _shouldDisplayMessage; }
        //    set
        //    {
        //        _shouldDisplayMessage = value;
        //        NotifyOfPropertyChange(() => this.ShouldDisplayMessage);
        //    }
        //}

        public void ShowMessage(ErrorMode errorMode, string message)
        {
            this.ErrorMode = errorMode;
            this.MessageText = message;
            //this.ShouldDisplayMessage = true;

            ////reset the interval each time a message is displayed
            //_timer.Interval = 5000;
            //_timer.Start();
        }
    }
}
using ZuneSocialTagger.GUIV2.Models;
using System;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class InlineZuneMessageViewModel : NotifyPropertyChangedImpl
    {
        private string _messageText;
        private ErrorMode _errorMode;

        public event Action DoShowMessage = delegate { };

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

            DoShowMessage.Invoke();
        }
    }
}
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class InlineZuneMessageViewModel : NotifyPropertyChangedImpl
    {
        private string _messageText;
        private ErrorMode _errorMode;
        private bool _visibility;

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


        public bool Visibile
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                NotifyOfPropertyChange(() => this.Visibile);
            }
        }

        public void ShowMessage(ErrorMode errorMode, string message)
        {
            this.ErrorMode = errorMode;
            this.MessageText = message;
            this.Visibile = true;
        }
    }
}
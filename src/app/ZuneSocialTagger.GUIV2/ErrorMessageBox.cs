using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2
{
    public class ErrorMessageBox
    {
        private  string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public void Show(string errorMessage)
        {
            _errorMessage = errorMessage;
            var errorMessageBoxView = new ErrorMessageBoxView(this);
            errorMessageBoxView.Show();
        }
    }
}
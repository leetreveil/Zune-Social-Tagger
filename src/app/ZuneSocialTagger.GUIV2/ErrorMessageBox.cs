using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2
{
    public static class ErrorMessageBox
    {
        public static void Show(string errorMessage)
        {
            new ErrorMessageBoxView(errorMessage).Show();
        }
    }
}
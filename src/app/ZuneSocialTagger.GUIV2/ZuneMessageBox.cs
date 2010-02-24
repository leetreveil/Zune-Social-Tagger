using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2
{
    public static class ZuneMessageBox
    {
        public static void Show(string message,ErrorMode errorMode)
        {
            new ZuneMessageBoxView(message,errorMode).Show();
        }
    }
}
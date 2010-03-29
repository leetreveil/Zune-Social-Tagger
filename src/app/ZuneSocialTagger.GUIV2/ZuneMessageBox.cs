using System;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;

namespace ZuneSocialTagger.GUIV2
{
    public static class ZuneMessageBox
    {
        public static void Show(ErrorMessage message)
        {
            new ZuneMessageBoxView(message.Message,message.ErrorMode).Show();
        }
        public static void Show(ErrorMessage message,ZuneMessageBoxButton buttonMode, Action okClickedCallback)
        {
            new ZuneMessageBoxView(message.Message, message.ErrorMode,buttonMode,okClickedCallback).Show();
        }
    }
}
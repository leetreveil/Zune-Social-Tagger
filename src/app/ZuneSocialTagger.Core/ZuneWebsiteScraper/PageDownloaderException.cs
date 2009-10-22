using System;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public class PageDownloaderException : Exception
    {
        public PageDownloaderException(string message) : base(message){}
        public PageDownloaderException(string message, Exception innerException) : base(message,innerException){}
    }
}
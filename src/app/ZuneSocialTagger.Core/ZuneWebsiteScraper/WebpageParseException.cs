using System;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public class WebpageParseException : Exception
    {
        public WebpageParseException(string message) : base(message)
        {
            
        }
    }
}
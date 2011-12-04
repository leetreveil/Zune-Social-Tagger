using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuneSocialTagger.GUI
{
    public static class AppSettings
    {
        public static string UpdateFeedUrl { get { return @"https://github.com/leetreveil/Zune-Social-Tagger/raw/master/updates/zunesocupdatefeed.xml"; } }
        public static string AppBaseUrl { get { return "http://github.com/leetreveil/Zune-Social-Tagger"; } }
        public static string AppDataFolder { get; set; }
    }
}

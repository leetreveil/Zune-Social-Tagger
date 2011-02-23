namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public static class Urls
    {
        public static string Album = string.Format("http://catalog.zune.net/v3.2/{0}/music/album/", Culture);
        public static string Artist = string.Format("http://catalog.zune.net/v3.2/{0}/music/artist/", Culture);
        public static string Image = "http://image.catalog.zune.net/v3.2/image/";
        public static string Schema = "http://schemas.zune.net/catalog/music/2007/10";

        private static string Culture
        {
            get { return Locale.GetLocale(); }

        }
    }
}
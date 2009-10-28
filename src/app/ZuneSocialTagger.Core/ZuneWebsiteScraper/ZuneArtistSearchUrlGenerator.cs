using System;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public static class ZuneArtistSearchUrlGenerator
    {
        public static string CreateUrl(string artist, int pageIndex)
        {
            return String.Format(
                "http://social.zune.net/frag/AlbumSearchBlock/?PageIndex={0}&IsFullListView=true&keyword={1}&PageSize=&blockName=AlbumSearchBlock&id=_searchAlbums&",
                pageIndex, artist);
        }

        public static string CreateUrl(string artist)
        {
            return CreateUrl(artist, 1);
        }
    }
}
namespace ZuneSocialTagger.Core
{
    public static class MediaIds
    {
        public static string[] Ids
        { 
            get
            { 
                return new[]{ZuneAlbumArtistMediaID,ZuneAlbumMediaID,ZuneMediaID};
            }
        }

        public static string ZuneAlbumArtistMediaID { get { return "ZuneAlbumArtistMediaID"; } }
        public static string ZuneAlbumMediaID { get { return "ZuneAlbumMediaID"; } }
        public static string ZuneMediaID { get { return "ZuneMediaID"; } }
    }
}
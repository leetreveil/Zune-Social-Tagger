namespace ZuneSocialTagger.Core
{
    /// <summary>
    /// These are the id's that the zune software uses to map a track to a certain artist, album and track
    /// </summary>
    public static class ZuneAttributes
    {
        public static string Artist = "ZuneAlbumArtistMediaID";
        public static string Album = "ZuneAlbumMediaID";
        public static string Track = "ZuneMediaID";

        public static string[] Ids
        {
            get { return new[] { Artist, Album, Track }; }
        }
    }
}
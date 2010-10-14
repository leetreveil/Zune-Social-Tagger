namespace ZuneSocialTagger.Core.IO
{
    /// <summary>
    /// These are the id's that the zune software uses to map a track to a certain artist, album and track
    /// </summary>
    public static class ZuneIds
    {
        public static string Artist = "ZuneAlbumArtistMediaID";
        public static string Album = "ZuneAlbumMediaID";
        public static string Track = "ZuneMediaID";

        public static string[] GetAll
        {
            get { return new[] { Artist, Album, Track }; }
        }
    }
}
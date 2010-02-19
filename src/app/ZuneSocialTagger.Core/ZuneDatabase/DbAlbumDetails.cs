using System;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class DbAlbumDetails
    {
        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }
        public string ArtworkUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public string AlbumMediaId { get; set; }

        /// <summary>
        /// This maps to MediaID in the zune library but to reduce confusion its named as Index
        /// </summary>
        public int Index { get; set; }
    }
}
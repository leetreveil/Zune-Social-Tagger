using System;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumDetails
    {
        public int MediaId { get; set; }
        public string AlbumMediaId { get; set; }
        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }
        public DateTime DateAdded { get; set; }
        public string ArtworkUrl { get; set; }
        public int TrackCount { get; set; }
        public int ReleaseYear { get; set; }
    }
}
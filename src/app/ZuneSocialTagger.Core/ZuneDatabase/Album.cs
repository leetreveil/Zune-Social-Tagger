using System;
using System.Collections.Generic;


namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class Album
    {
        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }
        public string ArtworkUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public Guid AlbumMediaId { get; set; }
        public int MediaId { get; set; }
        public int ReleaseYear { get; set; }
        public int TrackCount { get; set; }
        public List<Track> Tracks { get; set; }
    }
}
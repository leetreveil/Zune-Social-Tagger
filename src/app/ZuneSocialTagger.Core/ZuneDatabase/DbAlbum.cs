using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    [Serializable]
    public class DbAlbum
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtworkUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public Guid AlbumMediaId { get; set; }
        public int MediaId { get; set; }
        public string ReleaseYear { get; set; }
        public int TrackCount { get; set; }
        public List<DbTrack> Tracks { get; set; }
    }
}
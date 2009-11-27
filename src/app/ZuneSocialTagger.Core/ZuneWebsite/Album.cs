using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class Album
    {
        public Guid AlbumMediaID { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public int AlbumReleaseYear { get; set; }
        public string AlbumArtworkUrl { get; set; }

        public bool IsValid
        {
            get { return AlbumMediaID != Guid.Empty && Tracks.AreAllValid(); }
        }
    }
}
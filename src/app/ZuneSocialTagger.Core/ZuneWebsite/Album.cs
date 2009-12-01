using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class Album
    {
        public Guid AlbumMediaID { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public int? ReleaseYear { get; set; }
        public string ArtworkUrl { get; set; }

        public bool IsValid
        {
            get { return AlbumMediaID != Guid.Empty && Tracks.AreAllValid(); }
        }
    }
}
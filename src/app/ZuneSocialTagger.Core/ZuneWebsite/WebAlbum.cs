using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    [Serializable]
    public class WebAlbum
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtworkUrl { get; set; }
        public Guid AlbumMediaId { get; set; }
        public string ReleaseYear { get; set; }
        public IEnumerable<WebTrack> Tracks { get; set; }
        public string Genre { get; set; }
    }
}
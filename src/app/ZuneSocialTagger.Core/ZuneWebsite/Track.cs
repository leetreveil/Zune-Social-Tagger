using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class Track
    {
        public Guid AlbumMediaID { get; set; }
        public Guid ArtistMediaID { get; set; }
        public Guid MediaID { get; set; }
        public string ArtworkUrl { get; set; }
        public MetaData MetaData { get; set; }

        public bool IsValid
        {
            get
            {
                //A songGuid is valid if its guid is not empty and its title is not empty or null
                return MediaID != Guid.Empty && ArtistMediaID != Guid.Empty && AlbumMediaID != Guid.Empty;
            }
        }
    }

    public class MetaData
    {
        public string Genre { get; set; }
        public string DiscNumber { get; set; }
        public string AlbumName { get; set; }
        public string Year { get; set; }
        public string Title { get; set; }
        public string AlbumArtist { get; set; }
        public string TrackNumber { get; set; }
        public IEnumerable<String> ContributingArtists { get; set; }
    }
}
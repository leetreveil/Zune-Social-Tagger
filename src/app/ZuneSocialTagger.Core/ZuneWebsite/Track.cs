using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class Track
    {
        public Guid ArtistMediaID { get; set; }
        public Guid MediaID { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public int? TrackNumber { get; set; }
        public IEnumerable<String> ContributingArtists { get; set; }
        public string Genre { get; set; }
        public int? DiscNumber { get; set; }

        public bool IsValid()
        {
            //A songGuid is valid if its guid is not empty and its title is not empty or null
            return MediaID != Guid.Empty && 
                   ArtistMediaID != Guid.Empty && 
                   !String.IsNullOrEmpty(Title);
        }
    }
}
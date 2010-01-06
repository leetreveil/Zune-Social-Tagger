using System;

namespace ZuneSocialTagger.Core
{
    public class Track
    {
        public Guid AlbumMediaID { get; set; }
        public Guid ArtistMediaID { get; set; }
        public Guid MediaID { get; set; }
        public string ArtworkUrl { get; set; }
        public MetaData MetaData { get; set; }

        public bool HasAllMediaIDs
        {
            get
            {
                //A songGuid is valid if its guid is not empty and its title is not empty or null
                return MediaID != Guid.Empty && ArtistMediaID != Guid.Empty && AlbumMediaID != Guid.Empty;
            }
        }
    }
}
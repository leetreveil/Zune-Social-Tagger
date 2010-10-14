using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    [Serializable]
    public class WebTrack
    {
        public Guid AlbumMediaId { get; set; }
        public Guid ArtistMediaId { get; set; }
        public Guid MediaId { get; set; }
        public string Title { get; set; }
        public string TrackNumber { get; set; }
        public string DiscNumber { get; set; }
        public string Genre { get; set; }
        public List<string> ContributingArtists { get; set; }
        public string Artist { get; set; }

        [XmlIgnore]
        public bool HasAllZuneIds
        {
            get
            {
                //A songGuid is valid if its guid is not empty and its title is not empty or null
                return MediaId != Guid.Empty && ArtistMediaId != Guid.Empty && AlbumMediaId != Guid.Empty;
            }
        }
    }
}
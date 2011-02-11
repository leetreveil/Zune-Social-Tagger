using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ProtoBuf;
using System.Diagnostics;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    [ProtoContract]
    public class WebTrack
    {
        public WebTrack()
        {
            Trace.WriteLine("hit");
        }
        [ProtoMember(1)]
        public Guid AlbumMediaId { get; set; }
        [ProtoMember(2)]
        public Guid ArtistMediaId { get; set; }
        [ProtoMember(3)]
        public Guid MediaId { get; set; }
        [ProtoMember(4)]
        public string Title { get; set; }
        [ProtoMember(5)]
        public string TrackNumber { get; set; }
        [ProtoMember(6)]
        public string DiscNumber { get; set; }
        [ProtoMember(7)]
        public string Genre { get; set; }
        [ProtoMember(8)]
        public List<string> ContributingArtists { get; set; }
        [ProtoMember(9)]
        public string Artist { get; set; }

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
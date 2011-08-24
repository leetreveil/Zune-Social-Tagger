using System;
using System.Collections.Generic;
using ProtoBuf;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    [ProtoContract]
    public class DbAlbum
    {
        [ProtoMember(1)]
        public string Title { get; set; }
        [ProtoMember(2)]
        public string Artist { get; set; }
        [ProtoMember(3)]
        public string ArtworkUrl { get; set; }
        [ProtoMember(4)]
        public DateTime DateAdded { get; set; }
        [ProtoMember(5)]
        public Guid AlbumMediaId { get; set; }
        [ProtoMember(6)]
        public int MediaId { get; set; }
        [ProtoMember(7)]
        public string ReleaseYear { get; set; }
        [ProtoMember(8)]
        public int TrackCount { get; set; }
        [ProtoMember(9)]
        public List<DbTrack> Tracks { get; set; }
    }
}
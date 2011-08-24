using System;
using System.Collections.Generic;
using ProtoBuf;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    [ProtoContract]
    public class WebAlbum
    {
        [ProtoMember(1)]
        public string Title { get; set; }
        [ProtoMember(2)]
        public string Artist { get; set; }
        [ProtoMember(3)]
        public string ArtworkUrl { get; set; }
        [ProtoMember(4)]
        public Guid AlbumMediaId { get; set; }
        [ProtoMember(5)]
        public string ReleaseYear { get; set; }
        [ProtoMember(6)]
        public List<WebTrack> Tracks { get; set; }
        [ProtoMember(7)]
        public string Genre { get; set; }
    }
}
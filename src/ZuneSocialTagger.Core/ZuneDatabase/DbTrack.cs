using System;
using ProtoBuf;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    [ProtoContract]
    public class DbTrack
    {
        [ProtoMember(1)]
        public Guid MediaId { get; set; }
        [ProtoMember(2)]
        public string FilePath { get; set; }
        [ProtoMember(3)]
        public string Title { get; set; }
        [ProtoMember(4)]
        public string TrackNumber { get; set; }
    }
}
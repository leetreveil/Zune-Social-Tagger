using System;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;
using ProtoBuf;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.Models
{
    /// <summary>
    /// The minimum amount of data required for the cache to do its job
    /// </summary>
    [ProtoContract]
    public class MinCache
    {
        [ProtoMember(1)]
        public Guid AlbumMediaId { get; set; }
        [ProtoMember(2)]
        public LinkStatus LinkStatus { get; set; }
        [ProtoMember(3)]
        public AlbumThumbDetails Right { get; set; }
    }
}

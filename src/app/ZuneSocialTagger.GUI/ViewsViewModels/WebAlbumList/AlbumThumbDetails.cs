using ProtoBuf;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    [ProtoContract]
    public class AlbumThumbDetails
    {
        [ProtoMember(1)]
        public string Artist { get; set; }
        [ProtoMember(2)]
        public string Title { get; set; }
        [ProtoMember(3)]
        public string ArtworkUrl { get; set; }
    }
}

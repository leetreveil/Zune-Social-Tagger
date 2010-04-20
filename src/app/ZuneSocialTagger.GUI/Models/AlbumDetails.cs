using ZuneSocialTagger.Core.ZuneDatabase;

namespace ZuneSocialTagger.GUI.Models
{
    public class AlbumDetails
    {
        public Album ZuneAlbumMetaData { get; set; }
        public Album WebAlbumMetaData { get; set; }
        public LinkStatus LinkStatus { get; set; }
    }
}
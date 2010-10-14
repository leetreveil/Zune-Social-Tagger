using System;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.Models
{
    [Serializable]
    public class SerializedAlbum
    {
        public SerializedAlbum(DbAlbum dbAlbum, WebAlbum webAlbum, LinkStatus linkStatus)
        {
            DbAlbum = dbAlbum;
            WebAlbum = webAlbum;
            LinkStatus = linkStatus;
        }

        public DbAlbum DbAlbum { get; set; }
        public WebAlbum WebAlbum { get; set; }
        public LinkStatus LinkStatus { get; set; }
    }
}
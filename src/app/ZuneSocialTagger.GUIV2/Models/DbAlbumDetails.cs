using System;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class DbAlbumDetails : AlbumMetaData
    {
        public DateTime DateAdded { get; set; }
        public string AlbumMediaId { get; set; }
    }
}
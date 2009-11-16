using System;
using System.Drawing;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class MetaData
    {
        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public string Year { get; set; }
        public string SongTitle { get; set; }
        public Image Picture { get; set; }
        public string Index { get; set; }
    }
}
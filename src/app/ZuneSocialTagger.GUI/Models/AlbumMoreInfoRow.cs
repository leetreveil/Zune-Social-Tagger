using System;

namespace ZuneSocialTagger.GUI.Models
{
    public class AlbumMoreInfoRow
    {
        public DetailRowSong TrackFromFile { get; set; }
        public Uri LinkStatusImage { get; set; }
        public string LinkStatusText { get; set; }
        public DetailRowSong TrackFromWeb { get; set; }
    }
}
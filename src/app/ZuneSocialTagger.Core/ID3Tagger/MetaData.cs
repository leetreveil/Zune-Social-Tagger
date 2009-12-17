using System.Drawing;
using System;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class MetaData
    {
        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public string Year { get; set; }
        public string SongTitle { get; set; }
        public Image Picture { get; set; }
        public string TrackNumber { get; set; }
        public string ContributingArtist { get; set; }
        public string DiscNumber { get; set; }
        public string Genre { get; set; }


        /// <summary>
        /// A MetaData object is valid if everything is not null or empty apart from the picture && genre && contributing artist, which can be
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(Year) && !String.IsNullOrEmpty(TrackNumber) &&
                       !String.IsNullOrEmpty(SongTitle) &&
                       !String.IsNullOrEmpty(DiscNumber) &&
                       !String.IsNullOrEmpty(AlbumTitle) && !String.IsNullOrEmpty(AlbumArtist);
            }
        }
    }
}
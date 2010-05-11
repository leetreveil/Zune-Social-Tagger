using System.Collections.Generic;
using System.Linq;

namespace ZuneSocialTagger.GUI.Models
{
    public class DetailRow
    {
        public DetailRow()
        {
            this.AvailableZuneTracks = new List<DetailRowSong>();
        }

        public DetailRowSong SongDetails { get; set; }
        public List<DetailRowSong> AvailableZuneTracks { get; set; }
        public DetailRowSong SelectedSong { get; set; }

        /// <summary>
        /// Matches song titles, only matches if the titles are exactly the same, needs extending
        /// </summary>
        /// <returns></returns>
        public void MatchTheSelectedSongToTheAvailableSongs()
        {
            //this matches album songs to zune website songs in the details view
            //Hold Your Colour ---- hold your colour (Album) = MATCH
            //Hold your colour ---- hold your face = NO MATCH
            DetailRowSong foundSongTitle =
                this.AvailableZuneTracks.Where(
                    song => song.TrackTitle.ToLower().Contains(this.SongDetails.TrackTitle.ToLower())).FirstOrDefault();

            this.SelectedSong = foundSongTitle ?? new DetailRowSong();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Details
{
    public class DetailRow
    {
        public DetailRow()
        {
            AvailableZuneTracks = new List<TrackWithTrackNum>();
        }

        public TrackWithTrackNum SongDetails { get; set; }
        public List<TrackWithTrackNum> AvailableZuneTracks { get; set; }
        public TrackWithTrackNum SelectedSong { get; set; }

        /// <summary>
        /// Matches song titles, only matches if the titles are exactly the same, needs extending
        /// </summary>
        /// <returns></returns>
        public void MatchTheSelectedSongToTheAvailableSongs()
        {
            //this matches album songs to zune website songs in the details view
            //Hold Your Colour ---- hold your colour (Album) = MATCH
            //Hold your colour ---- hold your face = NO MATCH
            this.SelectedSong = AvailableZuneTracks.Where(song => song.TrackTitle.ToLower()
                    .Contains(SongDetails.TrackTitle.ToLower()))
                    .FirstOrDefault();
        }
    }
}
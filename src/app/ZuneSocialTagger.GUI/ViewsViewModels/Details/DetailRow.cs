using System.Collections.Generic;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Details
{
    public class DetailRow
    {
        public DetailRow()
        {
            this.AvailableZuneTracks = new List<TrackWithTrackNum>();
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
            this.SelectedSong = SharedMethods.GetMatchingTrack(this.AvailableZuneTracks, this.SongDetails);
        }
    }
}
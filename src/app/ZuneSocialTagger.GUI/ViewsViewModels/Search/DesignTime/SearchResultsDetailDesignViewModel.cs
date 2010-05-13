using System.Collections.Generic;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search.DesignTime
{
    public class SearchResultsDetailDesignViewModel
    {
        public SearchResultsDetailDesignViewModel()
        {
            this.SelectedAlbumSongs = new List<TrackWithTrackNum>();

            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "1", TrackTitle = "Prelude" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "2", TrackTitle = "Slam" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "3", TrackTitle = "Plasticworld" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "4", TrackTitle = "Fasten Your Seatbelt" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "5", TrackTitle = "Through The Loop" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "6", TrackTitle = "Sounds Of Life" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "7", TrackTitle = "Girl In The Fire" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "8", TrackTitle = "Tarantula" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "9", TrackTitle = "Out Here" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "10", TrackTitle = "Hold Your Colour" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "11", TrackTitle = "The Terminal" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "12", TrackTitle = "Streamline" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "13", TrackTitle = "Another Planet" });
            this.SelectedAlbumSongs.Add(new TrackWithTrackNum { TrackNumber = "14", TrackTitle = "Still Grey" });
        }

        public string SelectedAlbumTitle { get { return "Hold Your Colour"; } }
        public List<TrackWithTrackNum> SelectedAlbumSongs { get; set; }
    }
}
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class SearchResultsDetailDesignViewModel
    {
        public SearchResultsDetailDesignViewModel()
        {
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "01", Title = "Prelude" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "02", Title = "Slam" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "03", Title = "Plasticworld" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "04", Title = "Fasten Your Seatbelt" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "05", Title = "Through The Loop" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "06", Title = "Sounds Of Life" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "07", Title = "Girl In The Fire" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "08", Title = "Tarantula" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "09", Title = "Out Here" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "10", Title = "Hold Your Colour" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "11", Title = "The Terminal" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "12", Title = "Streamline" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "13", Title = "Another Planet" });
            this.SelectedAlbumSongs.Add(new WebTrack { TrackNumber = "14", Title = "Still Grey" });
        }

        public string SelectedAlbumTitle { get { return "Hold Your Colour"; } }
        public List<WebTrack> SelectedAlbumSongs { get; set; }
    }
}
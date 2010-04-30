using System.Collections.Generic;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class SearchResultsDetailDesignViewModel
    {
        public SearchResultsDetailDesignViewModel()
        {
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "01", Title = "Prelude" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "02", Title = "Slam" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "03", Title = "Plasticworld" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "04", Title = "Fasten Your Seatbelt" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "05", Title = "Through The Loop" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "06", Title = "Sounds Of Life" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "07", Title = "Girl In The Fire" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "08", Title = "Tarantula" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "09", Title = "Out Here" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "10", Title = "Hold Your Colour" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "11", Title = "The Terminal" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "12", Title = "Streamline" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "13", Title = "Another Planet" });
            this.SelectedAlbumSongs.Add(new Track() { TrackNumber = "14", Title = "Still Grey" });
        }

        public string SelectedAlbumTitle { get { return "Hold Your Colour"; } }
        public List<Track> SelectedAlbumSongs { get; set; }
    }
}
using System.Collections.Generic;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class SearchResultsDetailDesignViewModel
    {
        public SearchResultsDetailDesignViewModel()
        {
            this.SelectedAlbumSongs = new List<DetailRowSong>();

            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "1", TrackTitle = "Prelude" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "2", TrackTitle = "Slam" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "3", TrackTitle = "Plasticworld" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "4", TrackTitle = "Fasten Your Seatbelt" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "5", TrackTitle = "Through The Loop" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "6", TrackTitle = "Sounds Of Life" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "7", TrackTitle = "Girl In The Fire" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "8", TrackTitle = "Tarantula" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "9", TrackTitle = "Out Here" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "10", TrackTitle = "Hold Your Colour" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "11", TrackTitle = "The Terminal" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "12", TrackTitle = "Streamline" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "13", TrackTitle = "Another Planet" });
            this.SelectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "14", TrackTitle = "Still Grey" });
        }

        public string SelectedAlbumTitle { get { return "Hold Your Colour"; } }
        public List<DetailRowSong> SelectedAlbumSongs { get; set; }
    }
}
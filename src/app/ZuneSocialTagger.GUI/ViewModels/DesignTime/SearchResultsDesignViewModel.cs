using System.Collections.ObjectModel;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class SearchResultsDesignViewModel
    {
        public ObservableCollection<object> SearchResults { get; set; }
        public SearchResultsDetailViewModel SearchResultsDetailViewModel { get; set; }
        public bool IsLoading { get { return true; } }
        public bool ArtistMode { get { return false; } }
        public string AlbumCount { get { return "ALBUMS (10)"; } }
        public string ArtistCount { get { return "ARTISTS (3)"; } }

        public SearchResultsDesignViewModel()
        {
            this.SearchResults = new ObservableCollection<object>();

            this.SearchResults.Add(new WebAlbum
            {
                Artist = "Pendulum",
                Title = "In Silico",
                ReleaseYear = "2009",
                ArtworkUrl = "http://img.noiset.com/images/album/pendulum-in-silico-cd-cover-artwork-1841.jpeg"
            });

            this.SearchResults.Add(new WebAlbum
            {
                Artist = "Pendulum",
                Title = "Hold Your Color",
                ReleaseYear = "2006",
                ArtworkUrl = "http://upload.wikimedia.org/wikipedia/en/thumb/e/ea/Pendulum-hold_your_colour.jpg/200px-Pendulum-hold_your_colour.jpg"
            });


            this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel();

            this.SearchResultsDetailViewModel.SelectedAlbumTitle = "Hold Your Colour";
            this.SearchResultsDetailViewModel.SelectedAlbumSongs = new ObservableCollection<WebTrack>();

            ObservableCollection<WebTrack> selectedAlbumSongs = this.SearchResultsDetailViewModel.SelectedAlbumSongs;
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "01", Title = "Prelude" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "02", Title = "Slam" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "03", Title = "Plasticworld" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "04", Title = "Fasten Your Seatbelt" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "05", Title = "Through The Loop" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "06", Title = "Sounds Of Life" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "07", Title = "Girl In The Fire" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "08", Title = "Tarantula" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "09", Title = "Out Here" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "10", Title = "Hold Your Colour" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "11", Title = "The Terminal" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "12", Title = "Streamline" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "13", Title = "Another Planet" });
            selectedAlbumSongs.Add(new WebTrack { TrackNumber = "14", Title = "Still Grey" });
        }
    }
}
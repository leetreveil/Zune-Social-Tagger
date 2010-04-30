using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
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

            this.SearchResults.Add(new Album()
            {
                Artist = "Pendulum",
                Title = "In Silico",
                ReleaseYear = "2009",
                ArtworkUrl = "http://img.noiset.com/images/album/pendulum-in-silico-cd-cover-artwork-1841.jpeg"
            });

            this.SearchResults.Add(new Album()
            {
                Artist = "Pendulum",
                Title = "Hold Your Color",
                ReleaseYear = "2006",
                ArtworkUrl = "http://upload.wikimedia.org/wikipedia/en/thumb/e/ea/Pendulum-hold_your_colour.jpg/200px-Pendulum-hold_your_colour.jpg"
            });


            this.SearchResultsDetailViewModel = new SearchResultsDetailViewModel();

            this.SearchResultsDetailViewModel.SelectedAlbumTitle = "Hold Your Colour";
            this.SearchResultsDetailViewModel.SelectedAlbumSongs = new ObservableCollection<Track>();

            ObservableCollection<Track> selectedAlbumSongs = this.SearchResultsDetailViewModel.SelectedAlbumSongs;
            selectedAlbumSongs.Add(new Track { TrackNumber = "01", Title = "Prelude" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "02", Title = "Slam" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "03", Title = "Plasticworld" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "04", Title = "Fasten Your Seatbelt" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "05", Title = "Through The Loop" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "06", Title = "Sounds Of Life" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "07", Title = "Girl In The Fire" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "08", Title = "Tarantula" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "09", Title = "Out Here" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "10", Title = "Hold Your Colour" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "11", Title = "The Terminal" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "12", Title = "Streamline" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "13", Title = "Another Planet" });
            selectedAlbumSongs.Add(new Track { TrackNumber = "14", Title = "Still Grey" });
        }
    }
}
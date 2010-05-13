using System.Collections.ObjectModel;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;

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
        public bool HasResults { get; set; }

        public SearchResultsDesignViewModel()
        {
            this.HasResults = true;
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
            this.SearchResultsDetailViewModel.SelectedAlbumSongs = new ObservableCollection<DetailRowSong>();

            ObservableCollection<DetailRowSong> selectedAlbumSongs = this.SearchResultsDetailViewModel.SelectedAlbumSongs;
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "1", TrackTitle = "Prelude" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "2", TrackTitle = "Slam" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "3", TrackTitle = "Plasticworld" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "4", TrackTitle = "Fasten Your Seatbelt" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "5", TrackTitle = "Through The Loop" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "6", TrackTitle = "Sounds Of Life" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "7", TrackTitle = "Girl In The Fire" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "8", TrackTitle = "Tarantula" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "9", TrackTitle = "Out Here" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "10", TrackTitle = "Hold Your Colour" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "11", TrackTitle = "The Terminal" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "12", TrackTitle = "Streamline" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "13", TrackTitle = "Another Planet" });
            selectedAlbumSongs.Add(new DetailRowSong { TrackNumber = "14", TrackTitle = "Still Grey" });
        }
    }
}
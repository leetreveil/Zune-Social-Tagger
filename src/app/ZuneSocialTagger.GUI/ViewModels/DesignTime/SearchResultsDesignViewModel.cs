using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class SearchResultsDesignViewModel
    {
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


            var track1 = new Track() {MetaData = new MetaData() {TrackNumber = "01", Title = "Prelude"}};
            var track2 = new Track() { MetaData = new MetaData() { TrackNumber = "02", Title = "Slam" } };
            var track3 = new Track() { MetaData = new MetaData() { TrackNumber = "03", Title = "Plasticworld" } };
            var track4 = new Track() { MetaData = new MetaData() { TrackNumber = "04", Title = "Fasten Your Seatbelt" } };
            var track5 = new Track() { MetaData = new MetaData() { TrackNumber = "05", Title = "Through The Loop" } };
            var track6 = new Track() { MetaData = new MetaData() { TrackNumber = "06", Title = "Sounds Of Life" } };
            var track7 = new Track() { MetaData = new MetaData() { TrackNumber = "07", Title = "Girl In The Fire" } };
            var track8 = new Track() { MetaData = new MetaData() { TrackNumber = "08", Title = "Tarantula" } };
            var track9 = new Track() { MetaData = new MetaData() { TrackNumber = "09", Title = "Out Here" } };  
            var track10 = new Track() { MetaData = new MetaData() { TrackNumber = "09", Title = "Hold Your Colour" } };
            var track11 = new Track() { MetaData = new MetaData() { TrackNumber = "09", Title = "The Terminal" } };
            var track12 = new Track() { MetaData = new MetaData() { TrackNumber = "09", Title = "Streamline" } };
            var track13 = new Track() { MetaData = new MetaData() { TrackNumber = "09", Title = "Another Planet" } };
            var track14 = new Track() { MetaData = new MetaData() { TrackNumber = "09", Title = "Still Grey" } };

            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track1);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track2);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track3);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track4);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track5);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track6);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track7);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track8);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track9);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track10);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track11);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track12);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track13);
            this.SearchResultsDetailViewModel.SelectedAlbumSongs.Add(track14);
        }

        public ObservableCollection<object > SearchResults { get; set; }
        public SearchResultsDetailViewModel SearchResultsDetailViewModel { get; set; }
        public bool IsLoading { get { return true; } }
        public bool ArtistMode { get { return false; } }
        public string AlbumCount { get { return "ALBUMS (10)"; } }
        public string ArtistCount { get { return "ARTISTS (3)"; } }
    }
}
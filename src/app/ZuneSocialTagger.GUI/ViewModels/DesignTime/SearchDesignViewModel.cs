using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class SearchDesignViewModel
    {

        public SearchDesignViewModel()
        {
            this.AlbumDetails = new ExpandedAlbumDetailsViewModel()
                                    {
                                        Artist = "Pendulum",
                                        ArtworkUrl = "http://images.play.com/covers/12691916x.jpg",
                                        SongCount = "10",
                                        Title = "Immersion",
                                        Year = "2010"
                                    };

            this.SearchText = "Pendulum Immersion";
            this.SearchResultsViewModel = new SearchResultsViewModel(new ZuneWizardModel());
            
            this.SearchResultsViewModel.Albums.Add(new Album()
            {
                Artist = "Pendulum",
                Title = "In Silico",
                ReleaseYear = "2009",
                ArtworkUrl = "http://img.noiset.com/images/album/pendulum-in-silico-cd-cover-artwork-1841.jpeg"
            });

            this.SearchResultsViewModel.Albums.Add(new Album()
            {
                Artist = "Pendulum",
                Title = "Hold Your Color",
                ReleaseYear = "2006",
                ArtworkUrl = "http://upload.wikimedia.org/wikipedia/en/thumb/e/ea/Pendulum-hold_your_colour.jpg/200px-Pendulum-hold_your_colour.jpg"
            });


            SearchResultsDetailViewModel searchResultsDetailViewModel = this.SearchResultsViewModel.SearchResultsDetailViewModel = new SearchResultsDetailViewModel();


            searchResultsDetailViewModel.SelectedAlbumTitle = "In Silico";
            searchResultsDetailViewModel.SelectedAlbumSongs = new ObservableCollection<Track>();


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

            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track1);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track2);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track3);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track4);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track5);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track6);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track7);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track8);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track9);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track10);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track11);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track12);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track13);
            searchResultsDetailViewModel.SelectedAlbumSongs.Add(track14);
        }

        public ExpandedAlbumDetailsViewModel AlbumDetails { get; set; }
        public SearchResultsViewModel SearchResultsViewModel { get; set; }
        public string SearchText { get; set; }
    }

}
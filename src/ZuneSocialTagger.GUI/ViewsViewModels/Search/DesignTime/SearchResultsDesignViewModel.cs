using System.Collections.ObjectModel;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search.DesignTime
{
    public class SearchResultsDesignViewModel
    {
        public ObservableCollection<object> SearchResults { get; set; }
        public bool IsLoading { get; set; }
        public bool ArtistMode { get { return true; } }
        public string AlbumCount { get { return "ALBUMS (10)"; } }
        public string ArtistCount { get { return "ARTISTS (3)"; } }
        public bool HasResults { get; set; }

        public SearchResultsDesignViewModel()
        {
            this.IsLoading = true;
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
        }
    }
}
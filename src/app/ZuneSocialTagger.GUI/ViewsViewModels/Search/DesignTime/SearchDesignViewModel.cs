using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search.DesignTime
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

            this.SearchText = "Immersion Pendulum";
            this.CanShowResults = true;
            this.IsSearching = true;
        }

        public ExpandedAlbumDetailsViewModel AlbumDetails { get; set; }
        public string SearchText { get; set; }
        public bool CanShowResults { get; set; }
        public bool IsSearching { get; set; }
    }

}
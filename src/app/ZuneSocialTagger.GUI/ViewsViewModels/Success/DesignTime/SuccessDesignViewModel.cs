using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success.DesignTime
{
    public class SuccessDesignViewModel
    {
        public SuccessDesignViewModel()
        {
            this.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel
            {
                Artist = "Pendulum",
                SongCount = "10",
                Title = "Immersion",
                Year = "2010"
            };

            this.AlbumDetailsFromWebsite = new ExpandedAlbumDetailsViewModel
            {
                Artist = "Pendulum",
                ArtworkUrl = "http://images.play.com/covers/12691916x.jpg",
                SongCount = "10",
                Title = "Immersion",
                Year = "2010"
            };
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }
    }
}
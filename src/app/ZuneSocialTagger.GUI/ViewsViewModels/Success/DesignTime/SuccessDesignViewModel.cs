using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Success.DesignTime
{
    public class SuccessDesignViewModel
    {
        public SuccessDesignViewModel()
        {
            this.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel
            {
                Artist = "LeftArtist",
                SongCount = "10",
                Title = "LeftTitle",
                Year = "2010"
            };

            this.AlbumDetailsFromWebsite = new ExpandedAlbumDetailsViewModel
            {
                Artist = "RightArtist",
                SongCount = "10",
                Title = "RightTitle",
                Year = "2010"
            };
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }
    }
}
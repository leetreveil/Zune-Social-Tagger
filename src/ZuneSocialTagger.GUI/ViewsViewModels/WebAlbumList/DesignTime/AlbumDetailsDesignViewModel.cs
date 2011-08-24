using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList.DesignTime
{
    public class AlbumDetailsDesignViewModel
    {
        public LinkStatus LinkStatus { get; set; }
        public AlbumThumbDetails Right { get; set; }
        public AlbumThumbDetails Left { get; set; }
        public bool IsDownloadingDetails { get; set; }

        public AlbumDetailsDesignViewModel()
        {
            this.IsDownloadingDetails = true;
            this.LinkStatus = LinkStatus.Linked;
            this.Left = new AlbumThumbDetails
                    {
                        Artist = "AFI",
                        Title = "Sing the sorrow",
                    };
            this.Right = new AlbumThumbDetails
                    {
                        Artist = "U2",
                        Title = "Are shit",
                    };
        }
    }
}
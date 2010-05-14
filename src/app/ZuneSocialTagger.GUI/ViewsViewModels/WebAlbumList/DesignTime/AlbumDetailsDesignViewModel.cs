using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList.DesignTime
{
    public class AlbumDetailsDesignViewModel
    {
        public LinkStatus LinkStatus { get; set; }
        public WebAlbum WebAlbumMetaData { get; set; }
        public DbAlbum ZuneAlbumMetaData { get; set; }
        public bool IsDownloadingAlbumDetails { get; set; }

        public AlbumDetailsDesignViewModel()
        {
            this.IsDownloadingAlbumDetails = true;
            this.LinkStatus = LinkStatus.Linked;
            this.ZuneAlbumMetaData = new DbAlbum
                    {
                        Artist = "AFI",
                        Title = "Sing the sorrow",
                        ReleaseYear = "2010",
                    };
            this.WebAlbumMetaData = new WebAlbum
                    {
                        Artist = "AFI",
                        Title = "Sing the Sorrow",
                        ReleaseYear = "2008"
                    };
        }
    }
}
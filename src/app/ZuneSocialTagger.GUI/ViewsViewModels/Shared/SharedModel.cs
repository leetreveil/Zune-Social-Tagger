using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.Core.IO;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    public class SharedModel
    {
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWeb { get; set; }
        public IList<IZuneTagContainer> SongsFromFile { get; set; }
        public WebAlbum WebAlbum { get; set; }
    }
}
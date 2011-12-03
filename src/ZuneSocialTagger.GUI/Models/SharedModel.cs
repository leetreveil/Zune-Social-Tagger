using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.GUI.Shared;

namespace ZuneSocialTagger.GUI.Models
{
    public class SharedModel
    {  
        public SharedModel()
        {
            SongsFromFile = new List<IZuneTagContainer>();
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWeb { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public IList<IZuneTagContainer> SongsFromFile { get; set; }
        public WebAlbum WebAlbum { get; set; }
        public DbAlbum DbAlbum { get; set; }
    }
}
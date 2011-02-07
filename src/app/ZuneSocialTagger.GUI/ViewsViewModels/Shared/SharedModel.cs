using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.Core.IO;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Shared
{
    public class SharedModel
    {
        private ExpandedAlbumDetailsViewModel _albumDetailsFromFile;
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile
        {
            get
            {
                if (_albumDetailsFromFile == null)
                    return DbAlbum.GetAlbumDetailsFrom();
                
                return _albumDetailsFromFile;
            }
            set { _albumDetailsFromFile = value; }
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWeb { get; set; }
        public IList<IZuneTagContainer> SongsFromFile { get; set; }
        public WebAlbum WebAlbum { get; set; }
        public DbAlbum DbAlbum { get; set; }
    }
}
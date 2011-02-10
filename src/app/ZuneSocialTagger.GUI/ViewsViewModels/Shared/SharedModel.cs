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
                    _albumDetailsFromFile = DbAlbum.GetAlbumDetailsFrom();
                
                return _albumDetailsFromFile;
            }
            set { _albumDetailsFromFile = value; }
        }

        private ExpandedAlbumDetailsViewModel _albumDetailsFromWeb;
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWeb
        {
            get
            {
                if (_albumDetailsFromWeb == null)
                    _albumDetailsFromWeb = WebAlbum.GetAlbumDetailsFrom();

                return _albumDetailsFromWeb;
            }
            set { _albumDetailsFromWeb = value; }
        }

        public IList<IZuneTagContainer> SongsFromFile { get; set; }
        public WebAlbum WebAlbum { get; set; }
        public DbAlbum DbAlbum { get; set; }
    }
}
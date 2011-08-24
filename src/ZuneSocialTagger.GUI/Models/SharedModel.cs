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

        private ExpandedAlbumDetailsViewModel _albumDetailsFromFile;
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile
        {
            get
            {
                if (DbAlbum != null)
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
                if (WebAlbum != null)
                    _albumDetailsFromWeb = WebAlbum.GetAlbumDetailsFrom();

                return _albumDetailsFromWeb;
            }
        }

        public IList<IZuneTagContainer> SongsFromFile { get; set; }
        public WebAlbum WebAlbum { get; set; }
        public DbAlbum DbAlbum { get; set; }
    }
}
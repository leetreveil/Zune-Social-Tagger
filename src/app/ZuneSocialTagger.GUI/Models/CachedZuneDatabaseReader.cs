using System.Collections.Generic;
using System.IO;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUI.Properties;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public class CachedZuneDatabaseReader
    {
        private List<AlbumDetailsViewModel> _deserializedAlbums;

        public bool CanInitialize
        {
            get { return true; }
        }

        public bool Initialize()
        {
            try
            {
                using (var fs = new FileStream(
                    Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.xml"), FileMode.Open))
                   
                    _deserializedAlbums = fs.XmlDeserializeFromStream<List<AlbumDetailsViewModel>>();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IEnumerable<AlbumDetailsViewModel> ReadAlbums()
        {
            return _deserializedAlbums;
        }
    }
}
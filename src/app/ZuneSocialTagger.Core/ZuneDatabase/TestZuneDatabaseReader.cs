using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class TestZuneDatabaseReader : IZuneDatabaseReader
    {
        private List<DbAlbumDetails> _deserializedAlbums;

        public void Load()
        {
            // load xml doc
        }

        public IEnumerable<DbAlbumDetails> ReadAlbums()
        {
            var xmlSerializer =
                new XmlSerializer(typeof(List<DbAlbumDetails>));

            _deserializedAlbums = (List<DbAlbumDetails>)
                 xmlSerializer.Deserialize(XmlReader.Create(new FileStream("zunedatabasecache.xml", FileMode.Open)));

            return _deserializedAlbums;
        }

        public DbAlbumDetails GetAlbum(int index)
        {
            if (_deserializedAlbums != null && _deserializedAlbums.Count > 0)
                return _deserializedAlbums.Where(x => x.Index == index).FirstOrDefault();

            return null;
        }
    }
}
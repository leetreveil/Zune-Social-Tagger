using System;
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

        public bool Initialize()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof (List<DbAlbumDetails>));

                XmlReader textReader = XmlReader.Create( new FileStream("zunedatabasecache.xml",FileMode.Open));

                _deserializedAlbums = (List<DbAlbumDetails>)xmlSerializer.Deserialize(textReader);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<DbAlbumDetails> ReadAlbums()
        {
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
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

                XmlReader textReader =
                    XmlReader.Create(new FileStream(@"ZuneDatabase\zunedatabasecache.xml", FileMode.Open));

                _deserializedAlbums = (List<DbAlbumDetails>) xmlSerializer.Deserialize(textReader);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<Album> ReadAlbums()
        {
            return _deserializedAlbums.Select(album => new Album()
                                                           {
                                                               AlbumArtist = album.AlbumArtist,
                                                               AlbumMediaId = album.AlbumMediaId,
                                                               AlbumTitle = album.AlbumTitle,
                                                               ArtworkUrl = album.ArtworkUrl,
                                                               DateAdded = album.DateAdded,
                                                               MediaId = album.MediaId,
                                                               ReleaseYear = album.ReleaseYear,
                                                               TrackCount = album.TrackCount
                                                           });
        }

        public Album GetAlbum(int index)
        {
            if (_deserializedAlbums != null && _deserializedAlbums.Count > 0)
            {
                return _deserializedAlbums.Where(x => x.MediaId == index).Select(album => new Album()
                                                                                              {
                                                                                                  AlbumArtist =
                                                                                                      album.AlbumArtist,
                                                                                                  AlbumMediaId =
                                                                                                      album.AlbumMediaId,
                                                                                                  AlbumTitle =
                                                                                                      album.AlbumTitle,
                                                                                                  ArtworkUrl =
                                                                                                      album.ArtworkUrl,
                                                                                                  DateAdded =
                                                                                                      album.DateAdded,
                                                                                                  MediaId =
                                                                                                      album.MediaId,
                                                                                                  ReleaseYear =
                                                                                                      album.ReleaseYear,
                                                                                                  TrackCount =
                                                                                                      album.TrackCount
                                                                                              }).First();
            }

            return null;
        }

        public IEnumerable<Track> GetTracksForAlbum(int index)
        {
            var albumDetails = _deserializedAlbums.Where(x => x.MediaId == index).First();

           return albumDetails.Tracks.Select(track => new Track() {FilePath = track.FilePath});
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class TestZuneDatabaseReader : IZuneDatabaseReader
    {
        private List<DbAlbumDetails> _deserializedAlbums;

        public bool Initialize()
        {
            try
            {
                using (var fs = new FileStream(@"ZuneDatabase\zunedatabasecache.xml", FileMode.Open))
                    _deserializedAlbums = fs.XmlDeserializeFromStream<List<DbAlbumDetails>>();

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
                return _deserializedAlbums.Where(x => x.MediaId == index).Select(album => new Album
                                                                                              {
                          AlbumArtist = album.AlbumArtist,
                          AlbumMediaId = album.AlbumMediaId,
                          AlbumTitle = album.AlbumTitle,
                          ArtworkUrl = album.ArtworkUrl,
                          DateAdded = album.DateAdded,
                          MediaId = album.MediaId,
                          ReleaseYear =  album.ReleaseYear,
                          TrackCount =  album.TrackCount
                      }).First();
            }

            return null;
        }

        public IEnumerable<Track> GetTracksForAlbum(int albumId)
        {
            var albumDetails = _deserializedAlbums.Where(x => x.MediaId == albumId).First();

           return albumDetails.Tracks.Select(track => new Track() {FilePath = track.FilePath});
        }

        public event Action FinishedReadingAlbums;
        public event Action<int, int> ProgressChanged;

        public Album GetAlbumByAlbumTitle(string albumTitle)
        {
            throw new NotImplementedException();
        }

        public bool DoesAlbumExist(int index)
        {
            throw new NotImplementedException();
        }

        public void RemoveAlbumFromDatabase(int albumId)
        {
            throw new NotImplementedException();
        }

        public void AddTrackToDatabase(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class TestZuneDatabaseReader : IZuneDatabaseReader
    {
        private List<Album> _deserializedAlbums;

        public bool Initialize()
        {
            using (var fs = new FileStream(@"ZuneDatabase\testzunedatabase.xml", FileMode.Open))
                _deserializedAlbums = fs.XmlDeserializeFromStream<List<Album>>();

                return true;
        }

        public IEnumerable<Album> ReadAlbums()
        {
            int counter = 0;

            foreach (var album in _deserializedAlbums)
            {
                counter++;
                yield return album;
                ProgressChanged.Invoke(counter, _deserializedAlbums.Count);
            }

            FinishedReadingAlbums.Invoke();
        }

        public Album GetAlbum(int index)
        {
            if (_deserializedAlbums != null && _deserializedAlbums.Count > 0)
            {
                return _deserializedAlbums.Where(x => x.MediaId == index).First();
            }

            return null;
        }

        public IEnumerable<Track> GetTracksForAlbum(int albumId)
        {
            var albumDetails = _deserializedAlbums.Where(x => x.MediaId == albumId).First();

           return albumDetails.Tracks.Select(track => new Track() {FilePath = track.FilePath});
        }

        public event Action FinishedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public Album GetAlbumByAlbumTitle(string albumTitle)
        {
            throw new NotImplementedException();
        }

        public bool DoesAlbumExist(int index)
        {
            return true;
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
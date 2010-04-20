using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class TestZuneDatabaseReader : IZuneDatabaseReader
    {
        private List<Album> _deserializedAlbums;

        public bool CanInitialize
        {
            get { return true; }
        }

        public event Action FinishedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };
        public event Action StartedReadingAlbums = delegate { };

        public bool Initialize()
        {
            try
            {
                using (var fs = new FileStream(@"ZuneDatabase\testzunedatabase.xml", FileMode.Open))
                    _deserializedAlbums = fs.XmlDeserializeFromStream<List<Album>>();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Album> ReadAlbums()
        {
            this.StartedReadingAlbums.Invoke();

            int counter = 0;

            foreach (var album in _deserializedAlbums)
            {
                counter++;
                yield return album;
                ProgressChanged.Invoke(counter, _deserializedAlbums.Count);
            }

            FinishedReadingAlbums.Invoke();
        }

        public IEnumerable<Album> GetNewAlbums(IEnumerable<int> albumIds)
        {
            yield return new Album
            {
                AlbumArtist = "Pendulum",
                AlbumTitle = "Immersion",
                DateAdded = DateTime.Now,
                ReleaseYear = 2010,
                TrackCount = 12
            };

            yield return new Album
            {
                AlbumArtist = "Circa Survive",
                AlbumTitle = "Blue Sky Noise",
                DateAdded = DateTime.Now,
                ReleaseYear = 2010,
                TrackCount = 12
            };
        }

        public IEnumerable<int> GetRemovedAlbums(IEnumerable<int> albumIds)
        {
            //idicate that the first two albums should be removed
            return albumIds.Take(2);
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
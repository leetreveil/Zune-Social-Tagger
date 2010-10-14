#if DEBUG


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class TestZuneDatabaseReader : IZuneDatabaseReader
    {
        private List<DbAlbum> _deserializedAlbums;

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
                {
                    var serializer = new XmlSerializer(typeof(List<DbAlbum>));
                    _deserializedAlbums = serializer.Deserialize(fs) as List<DbAlbum>;
                }
                   
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<DbAlbum> ReadAlbums()
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

        public IEnumerable<DbAlbum> GetNewAlbums(IEnumerable<int> albumIds)
        {
            yield return new DbAlbum
            {
                Artist = "Pendulum",
                Title = "Immersion",
                DateAdded = DateTime.Now,
                ReleaseYear = "2010",
                TrackCount = 12
            };

            yield return new DbAlbum
            {
                Artist = "Circa Survive",
                Title = "Blue Sky Noise",
                DateAdded = DateTime.Now,
                ReleaseYear = "2010",
                TrackCount = 12
            };
        }

        public IEnumerable<int> GetRemovedAlbums(IEnumerable<int> albumIds)
        {
            //idicate that the first two albums should be removed
            return albumIds.Take(2);
        }


        public DbAlbum GetAlbum(int index)
        {
            if (_deserializedAlbums != null && _deserializedAlbums.Count > 0)
            {
                return _deserializedAlbums.Where(x => x.MediaId == index).First();
            }

            return null;
        }

        public IEnumerable<DbTrack> GetTracksForAlbum(int albumId)
        {
            var albumDetails = _deserializedAlbums.Where(x => x.MediaId == albumId).First();

            return albumDetails.Tracks.Select(track => new DbTrack() {FilePath = track.FilePath});
        }

        public DbAlbum GetAlbumByAlbumTitle(string albumTitle)
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

#endif
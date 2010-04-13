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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="albumIds">A list of albums to check against</param>
        /// <returns>A list of albums with a flag identifying if they where added / removed from the database</returns>
        public Dictionary<Album, DbAlbumChanged> CheckForChanges(IEnumerable<Album> albumIds)
        {
            var dict = new Dictionary<Album, DbAlbumChanged>();

            //Voodoo People & When Your Heart Stops Beating
            dict.Add(albumIds.ElementAt(0), DbAlbumChanged.Removed);
            //The Autumn Effect
            dict.Add(albumIds.ElementAt(1), DbAlbumChanged.Removed);


            dict.Add(new Album()
                            {
                                AlbumArtist = "Pendulum",
                                AlbumTitle = "Immersion",
                                DateAdded = DateTime.Now,
                                ReleaseYear = 2010,
                                TrackCount = 12
                            },DbAlbumChanged.Added);

            dict.Add(new Album()
            {
                AlbumArtist = "Circa Survive",
                AlbumTitle = "Blue Sky Noise",
                DateAdded = DateTime.Now,
                ReleaseYear = 2010,
                TrackCount = 12
            },DbAlbumChanged.Added);

            return dict;
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

    public enum DbAlbumChanged
    {
        Added,
        Removed
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public enum DbAlbumChanged
    {
        Added,
        Removed
    }

    public class TestZuneDatabaseReader : IZuneDatabaseReader
    {
        private List<Album> _deserializedAlbums;

        public bool CanInitialize
        {
            get
            {
                return true;
            }
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
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="albumIds">A list of album id's to check against</param>
        ///// <returns>A list of albim ids with a flag identifying if they where added / removed from the database</returns>
        //public Dictionary<Guid,DbAlbumChanged> CheckForChanges(IEnumerable<Guid> albumIds)
        //{
        //    var dict = new Dictionary<Guid, DbAlbumChanged>();

        //    //get list of media id's
        //    IEnumerable<Guid> dbMediaIds = _deserializedAlbums.Select(x => x.AlbumMediaId);

        //    foreach (Guid albumId in albumIds)
        //    {
        //        if (!_deserializedAlbums.Where(x=> x.AlbumMediaId))
        //        {
                    
        //        }
        //    }
        //}

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
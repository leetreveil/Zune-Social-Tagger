using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZuneSocialTagger.Core;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class CachedZuneDatabaseReader : IZuneDbAdapter
    {
        private List<AlbumDetails> _deserializedAlbums;

        public event Action FinishedReadingAlbums = delegate { };
        public event Action StartedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public bool CanInitialize
        {
            get { return true; }
        }

        public bool Initialize()
        {
            try
            {
                using (var fs = new FileStream(@"zunesoccache.xml", FileMode.Open))
                    _deserializedAlbums = fs.XmlDeserializeFromStream<List<AlbumDetails>>();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IEnumerable<AlbumDetails> ReadAlbums()
        {
            StartedReadingAlbums.Invoke();

            int counter = 0;

            foreach (var album in _deserializedAlbums)
            {
                counter++;
                yield return album;

                ProgressChanged.Invoke(counter, _deserializedAlbums.Count);
            }

            FinishedReadingAlbums.Invoke();
        }

        public AlbumDetails GetAlbum(int index)
        {
            if (_deserializedAlbums != null && _deserializedAlbums.Count > 0)
            {
                return (from album in _deserializedAlbums
                        where album.ZuneAlbumMetaData.MediaId == index
                        select album).First();
            }

            return null;
        }

        public IEnumerable<Track> GetTracksForAlbum(int albumId)
        {
            var albumDetails =
                _deserializedAlbums.Select(x => x.ZuneAlbumMetaData).Where(x => x.MediaId == albumId).First();

            return albumDetails.Tracks.Select(track => new Track
                                                           {
                                                               FilePath = track.FilePath
                                                           });
        }

        public bool DoesAlbumExist(int index)
        {
            //just returning true because we're loading from xml, 
            //if the user edited the cache then it would fail
            return true;
        }

        public void Dispose()
        {
        }
    }
}
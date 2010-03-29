using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;
using ZuneSocialTagger.GUIV2.Properties;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class CachedZuneDatabaseReader
    {
        private List<AlbumDetailsXml> _deserializedAlbums;

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
                using (var fs = new FileStream(Path.Combine(Settings.Default.AppDataFolder, @"zunesoccache.xml"), FileMode.Open))
                    _deserializedAlbums = fs.XmlDeserializeFromStream<List<AlbumDetailsXml>>();
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
                yield return ToAlbumDetails(album);

                ProgressChanged.Invoke(counter, _deserializedAlbums.Count);
            }

            FinishedReadingAlbums.Invoke();
        }

        public AlbumDetails GetAlbum(int index)
        {
            if (_deserializedAlbums != null && _deserializedAlbums.Count > 0)
            {
                AlbumDetailsXml albumDetailsXml = (from album in _deserializedAlbums
                                                   where album.ZuneAlbumMetaData.MediaId == index
                                                   select album).First();
                return ToAlbumDetails(albumDetailsXml);
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

        public static AlbumDetails ToAlbumDetails(AlbumDetailsXml albumDetailsXml)
        {
            return new AlbumDetails
                       {
                           LinkStatus = albumDetailsXml.LinkStatus,
                           WebAlbumMetaData = ToAlbum(albumDetailsXml.WebAlbumMetaData),
                           ZuneAlbumMetaData = ToAlbum(albumDetailsXml.ZuneAlbumMetaData)
                       };
        }

        public static Track ToTrack(TrackXml track)
        {
            return new Track
            {
                FilePath = track.FilePath,
                MediaId = track.MediaId
            };
        }

        public static Core.ZuneDatabase.Album ToAlbum(AlbumXml album)
        {
            if (album != null)
            {
                return new Core.ZuneDatabase.Album()
                {
                    AlbumArtist = album.AlbumArtist,
                    AlbumMediaId = album.AlbumMediaId,
                    AlbumTitle = album.AlbumTitle,
                    ArtworkUrl = album.ArtworkUrl,
                    DateAdded = album.DateAdded,
                    MediaId = album.MediaId,
                    ReleaseYear = album.ReleaseYear,
                    TrackCount = album.TrackCount,
                    Tracks = album.Tracks.Select(ToTrack).ToList()
                };
            }

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core.ZuneDatabase;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneDbAdapter : IZuneDbAdapter
    {
        private readonly IZuneDatabaseReader _zuneDatabaseReader;

        public event Action FinishedReadingAlbums = delegate { };
        public event Action StartedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public ZuneDbAdapter(IZuneDatabaseReader zuneDatabaseReader)
        {
            _zuneDatabaseReader = zuneDatabaseReader;

            zuneDatabaseReader.ProgressChanged += (arg1, arg2) => this.ProgressChanged.Invoke(arg1, arg2);
            zuneDatabaseReader.FinishedReadingAlbums += () => this.FinishedReadingAlbums.Invoke();
        }

        public bool CanInitialize
        {
            get { return _zuneDatabaseReader.CanInitialize; }
        }

        public bool Initialize()
        {
            return _zuneDatabaseReader.Initialize();
        }

        public IEnumerable<AlbumDetails> ReadAlbums()
        {
            this.StartedReadingAlbums.Invoke();

            return from album in _zuneDatabaseReader.ReadAlbums()
                   select ToAlbumDetails(album);
        }

        public AlbumDetails GetAlbum(int index)
        {
            return ToAlbumDetails(_zuneDatabaseReader.GetAlbum(index));
        }

        public IEnumerable<Track> GetTracksForAlbum(int albumId)
        {
            return _zuneDatabaseReader.GetTracksForAlbum(albumId);
        }

        public bool DoesAlbumExist(int index)
        {
            return _zuneDatabaseReader.DoesAlbumExist(index);
        }

        public void Dispose()
        {
        }

        private static AlbumDetails ToAlbumDetails(Album album)
        {
            var albumDetails = new AlbumDetails();

            albumDetails.LinkStatus = album.AlbumMediaId == Guid.Empty ? LinkStatus.Unlinked : LinkStatus.Unknown;
            albumDetails.ZuneAlbumMetaData = album;

            return albumDetails;
        }
    }
}
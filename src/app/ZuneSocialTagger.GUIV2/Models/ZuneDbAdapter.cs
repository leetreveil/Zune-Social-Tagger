using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneDbAdapter : IZuneDbAdapter
    {
        private readonly IZuneDatabaseReader _zuneDatabaseReader;
        private readonly IUnityContainer _container;

        public event Action FinishedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public ZuneDbAdapter(IZuneDatabaseReader zuneDatabaseReader, IUnityContainer container)
        {
            _zuneDatabaseReader = zuneDatabaseReader;
            _container = container;

            zuneDatabaseReader.ProgressChanged += (arg1, arg2) => this.ProgressChanged.Invoke(arg1,arg2);
            zuneDatabaseReader.FinishedReadingAlbums += () => this.FinishedReadingAlbums.Invoke();
        }

        public bool Initialize()
        {
            return _zuneDatabaseReader.Initialize();
        }

        public IEnumerable<AlbumDetailsViewModel> ReadAlbums()
        {
            return from album
                       in _zuneDatabaseReader.ReadAlbums()
                   select ToAlbumDetailsViewModel(album);
        }


        public AlbumDetailsViewModel GetAlbum(int index)
        {
            return ToAlbumDetailsViewModel(_zuneDatabaseReader.GetAlbum(index));
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

        private AlbumDetailsViewModel ToAlbumDetailsViewModel(Album album)
        {
            return new AlbumDetailsViewModel(_container,this)
                       {
                           ZuneAlbumMetaData = album
                       };
        }
    }
}
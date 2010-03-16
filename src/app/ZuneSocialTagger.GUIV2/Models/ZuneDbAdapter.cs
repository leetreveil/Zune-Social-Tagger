using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneDbAdapter : IZuneDbAdapter
    {
        private readonly IZuneDatabaseReader _zuneDatabaseReader;
        private readonly IServiceLocator _locator;

        public event Action FinishedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public ZuneDbAdapter(IZuneDatabaseReader zuneDatabaseReader, IServiceLocator locator)
        {
            _zuneDatabaseReader = zuneDatabaseReader;
            _locator = locator;

            zuneDatabaseReader.ProgressChanged += (arg1, arg2) => this.ProgressChanged.Invoke(arg1,arg2);
            zuneDatabaseReader.FinishedReadingAlbums += () => this.FinishedReadingAlbums.Invoke();
        }

        public bool Initialize()
        {
            return _zuneDatabaseReader.Initialize();
        }

        public IEnumerable<AlbumDetailsViewModel> ReadAlbums()
        {
            return from album in _zuneDatabaseReader.ReadAlbums()
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
            var albumDetailsViewModel = _locator.GetInstance<AlbumDetailsViewModel>();

            albumDetailsViewModel.ZuneAlbumMetaData = album;

            return albumDetailsViewModel;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.PresentationFramework;
using Microsoft.Practices.ServiceLocation;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.Core;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class CachedZuneDatabaseReader : IZuneDbAdapter
    {
        private BindableCollection<AlbumDetailsViewModel> _deserializedAlbums;
        private IServiceLocator _locator;

        public event Action FinishedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public CachedZuneDatabaseReader(IServiceLocator locator)
        {
            _locator = locator;
        }

        public bool Initialize()
        {
            try
            {
                using (var fs = new FileStream(@"zunesoccache.xml", FileMode.Open))
                    _deserializedAlbums = fs.XmlDeserializeFromStream<BindableCollection<AlbumDetailsViewModel>>();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IEnumerable<AlbumDetailsViewModel> ReadAlbums()
        {
            int counter = 0;

            foreach (var album in _deserializedAlbums)
            {
                counter++;
                yield return Construct(album);

                ProgressChanged.Invoke(counter, _deserializedAlbums.Count);
            }

            FinishedReadingAlbums.Invoke();
        }


        private AlbumDetailsViewModel Construct(AlbumDetailsViewModel albumDetailsViewModel)
        {
            var detailsViewModel = _locator.GetInstance<AlbumDetailsViewModel>();

            detailsViewModel.ZuneAlbumMetaData = albumDetailsViewModel.ZuneAlbumMetaData;
            detailsViewModel.LinkStatus = albumDetailsViewModel.LinkStatus;
            detailsViewModel.WebAlbumMetaData = albumDetailsViewModel.WebAlbumMetaData;   
  
            return detailsViewModel;
        }

        public AlbumDetailsViewModel GetAlbum(int index)
        {
            if (_deserializedAlbums != null && _deserializedAlbums.Count > 0)
            {
                return (from album in _deserializedAlbums
                        where album.ZuneAlbumMetaData.MediaId == index
                        select Construct(album)).First();
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
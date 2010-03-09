using System;
using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumDownloaderWithProgressReporting
    {
        private readonly IEnumerable<AlbumDetailsViewModel> _albums;
        private int _downloadCounter;

        public event Action FinishedDownloadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public AlbumDownloaderWithProgressReporting(IEnumerable<AlbumDetailsViewModel> albums)
        {
            _albums = albums;
        }

        public void Start()
        {
            foreach (var album in _albums)
            {
                string fullUrlToAlbumXmlDetails =
                    String.Concat("http://catalog.zune.net/v3.0/en-US/music/album/",
                                  album.ZuneAlbumMetaData.AlbumMediaId);

                if (album.LinkStatus == LinkStatus.Unknown)
                {
                    var reader = new AlbumDetailsDownloader(fullUrlToAlbumXmlDetails);

                    reader.DownloadCompleted += dledAlbum =>
                        {
                            _downloadCounter++;

                            SetAlbumDetails(dledAlbum, album);


                            //TODO: don't like how we are doing progress reporting
                            var totalUnlinkedCount = _albums.Where(x => x.LinkStatus != LinkStatus.Unlinked).Count();

                            this.ProgressChanged.Invoke(_downloadCounter, totalUnlinkedCount);

                            if (_downloadCounter == totalUnlinkedCount)
                            {
                                this.FinishedDownloadingAlbums.Invoke();
                            }
                        };

                    reader.DownloadAsync();
                }
            }
        }

        private void SetAlbumDetails(AlbumMetaData dledAlbum, AlbumDetailsViewModel album)
        {
            if (dledAlbum == null)
                album.LinkStatus = LinkStatus.Unavailable;
            else
            {
                album.LinkStatus =
                    ZuneTypeConverters.GetAlbumLinkStatus(dledAlbum.AlbumTitle,
                                             dledAlbum.AlbumArtist,
                                             album.ZuneAlbumMetaData);

                album.WebAlbumMetaData = new AlbumDetails
                {
                    AlbumArtist = dledAlbum.AlbumArtist,
                    AlbumTitle = dledAlbum.AlbumTitle,
                    ArtworkUrl = dledAlbum.ArtworkUrl
                };
            }
        }
    }
}
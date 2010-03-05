using System;
using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumDownloaderWithProgressReporting
    {
        private readonly IEnumerable<Album> _albums;
        private int _downloadCounter;

        public event Action FinishedDownloadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public AlbumDownloaderWithProgressReporting(IEnumerable<Album> albums)
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

                    Album album1 = album;
                    reader.DownloadCompleted += dledAlbum =>
                                                    {
                                                        _downloadCounter++;

                                                        if (dledAlbum == null)
                                                            album1.LinkStatus = LinkStatus.Unavailable;
                                                        else
                                                        {
                                                            album1.LinkStatus =
                                                                Album.GetAlbumLinkStatus(dledAlbum.AlbumTitle,
                                                                                         dledAlbum.AlbumArtist,
                                                                                         album1.ZuneAlbumMetaData);

                                                            album1.WebAlbumMetaData = new AlbumDetails
                                                                  {
                                                                      AlbumArtist = dledAlbum.AlbumArtist,
                                                                      AlbumTitle =  dledAlbum.AlbumTitle,
                                                                      ArtworkUrl = dledAlbum.ArtworkUrl
                                                                  };
                                                        }

                                                        var totalUnlinkedCount =
                                                            _albums.Where(x => x.LinkStatus != LinkStatus.Unlinked).
                                                                Count();

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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.ViewModels;
using ZuneSocialTagger.Core.ZuneDatabase;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumDownloaderWithProgressReporting
    {
        private readonly IEnumerable<AlbumDetailsViewModel> _albums;
        private int _downloadCounter;

        public event Action FinishedDownloadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        private Dictionary<AlbumDetailsDownloader, LinkStatus> _downloadList;

        public AlbumDownloaderWithProgressReporting(IEnumerable<AlbumDetailsViewModel> albums)
        {
            _albums = albums;
            _downloadList = new Dictionary<AlbumDetailsDownloader,LinkStatus>();
        }

        public void StopDownloading()
        {
            ThreadPool.QueueUserWorkItem(_ =>
             {
                 //only stop the download of albums that are not unlinked
                 foreach (var downloader in _downloadList.Where(x=> x.Value != LinkStatus.Unlinked))
                     downloader.Key.Cancel();
             });
        }

        public void Start()
        {
            //only download albums that we do not know the details of
            foreach (var album in _albums.Where(x => x.LinkStatus == LinkStatus.Unknown))
            {
                string fullUrlToAlbumXmlDetails =
                    String.Concat(Urls.Album, album.ZuneAlbumMetaData.AlbumMediaId);

                var downloader = new AlbumDetailsDownloader(fullUrlToAlbumXmlDetails);

                AlbumDetailsViewModel album1 = album;

                _downloadList.Add(downloader,album1.LinkStatus);

                downloader.DownloadCompleted += (dledAlbum, state) =>
                    {
                        if (state != DownloadState.Cancelled)
                        {
                            _downloadCounter++;

                            SetAlbumDetails(dledAlbum, album1);

                            //TODO: don't like how we are doing progress reporting
                            var totalUnlinkedCount = _albums.Where(x => x.LinkStatus != LinkStatus.Unlinked).Count();

                            this.ProgressChanged.Invoke(_downloadCounter, totalUnlinkedCount);

                            if (_downloadCounter == totalUnlinkedCount)
                            {
                                this.FinishedDownloadingAlbums.Invoke();
                            }
                        }
                    };

                downloader.DownloadAsync();
            }
        }

        private void SetAlbumDetails(AlbumMetaData dledAlbum, AlbumDetailsViewModel album)
        {
            if (dledAlbum == null)
                album.LinkStatus = LinkStatus.Unavailable;
            else
            {
                Album metaData = album.ZuneAlbumMetaData;

                album.LinkStatus = SharedMethods.GetAlbumLinkStatus(dledAlbum.AlbumTitle,
                                                                    dledAlbum.AlbumArtist,
                                                                    metaData.AlbumTitle,
                                                                    metaData.AlbumArtist);
                album.WebAlbumMetaData = new Album
                                             {
                                                 AlbumArtist = dledAlbum.AlbumArtist,
                                                 AlbumTitle = dledAlbum.AlbumTitle,
                                                 ArtworkUrl = dledAlbum.ArtworkUrl
                                             };
            }
        }
    }
}
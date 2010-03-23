using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private List<AlbumDetailsDownloader> _downloadList;

        /// <summary>
        /// Only pass in albums into the ctor that you want downloading! Don't pass in albums already downloaded /
        /// albums that cant be downloaded
        /// </summary>
        /// <param name="albums"></param>
        public AlbumDownloaderWithProgressReporting(IEnumerable<AlbumDetailsViewModel> albums)
        {
            _albums = albums;
            _downloadList = new List<AlbumDetailsDownloader>();
        }

        public void StopDownloading()
        {
            ThreadPool.QueueUserWorkItem(_ =>
             {
                 //only stop the download of albums that are not unlinked
                 foreach (var downloader in _downloadList)
                     downloader.Cancel();
             });
        }

        public void Start()
        {
            bool hasBeenCancelled = false;

            int albumCount = _albums.Count();

            //only download albums that we do not know the details of
            foreach (var album in _albums.Where(x => x.LinkStatus == LinkStatus.Unknown))
            {
                string fullUrlToAlbumXmlDetails =
                    String.Concat(Urls.Album, album.ZuneAlbumMetaData.AlbumMediaId);

                var downloader = new AlbumDetailsDownloader(fullUrlToAlbumXmlDetails);

                AlbumDetailsViewModel album1 = album;

                //maintain a list of all the downloaders so they can be cancelled in the future
                _downloadList.Add(downloader);

                downloader.DownloadCompleted += (dledAlbum, state) =>
                    {
                        if (state != DownloadState.Cancelled)
                        {
                            if (!hasBeenCancelled)
                            {
                                _downloadCounter++;

                                SetAlbumDetails(dledAlbum, album1);

                                //TODO: don't like how we are doing progress reporting

                                this.ProgressChanged.Invoke(_downloadCounter, albumCount);

                                if (_downloadCounter == albumCount)
                                    this.FinishedDownloadingAlbums.Invoke();
                            }
                        }

                        if (state == DownloadState.Cancelled)
                        {
                            if (!hasBeenCancelled)
                                this.FinishedDownloadingAlbums.Invoke();

                            hasBeenCancelled = true;
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
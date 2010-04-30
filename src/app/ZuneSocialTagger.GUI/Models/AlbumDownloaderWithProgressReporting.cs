using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{

    public class AlbumDownloaderWithProgressReporting
    {
        private readonly IEnumerable<AlbumDetailsViewModel> _albums;
        private int _downloadCounter;
        private readonly List<AlbumDetailsDownloader> _downloadList;

        public event Action FinishedDownloadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

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
            int albumCount = _albums.Count();

            foreach (var album in _albums)
            {
                string fullUrlToAlbumXmlDetails =
                    String.Concat(Urls.Album, album.ZuneAlbumMetaData.AlbumMediaId);

                var downloader = new AlbumDetailsDownloader(fullUrlToAlbumXmlDetails);

                AlbumDetailsViewModel album1 = album;

                //maintain a list of all the downloaders so they can be cancelled in the future
                _downloadList.Add(downloader);

                downloader.DownloadCompleted += (dledAlbum, state) =>
                    {
                        if (state == DownloadState.Success)
                        {
                            _downloadCounter++;

                            SetAlbumDetails(dledAlbum, album1);

                            this.ProgressChanged.Invoke(_downloadCounter, albumCount);

                            if (_downloadCounter == albumCount)
                                this.FinishedDownloadingAlbums.Invoke();
                        }

                        if (state == DownloadState.Cancelled)
                        {
                            this.FinishedDownloadingAlbums.Invoke();
                            return;
                        }
                    };

                downloader.DownloadAsync();
            }
        }

        private void SetAlbumDetails(Album dledAlbum, AlbumDetailsViewModel album)
        {
            if (dledAlbum == null)
                album.LinkStatus = LinkStatus.Unavailable;
            else
            {
                Album metaData = album.ZuneAlbumMetaData;

                album.LinkStatus = SharedMethods.GetAlbumLinkStatus(dledAlbum.Title,
                                                                    dledAlbum.Artist,
                                                                    metaData.Title,
                                                                    metaData.Artist);

                album.WebAlbumMetaData = new Album
                                             {
                                                 Artist = dledAlbum.Artist,
                                                 Title = dledAlbum.Title,
                                                 ArtworkUrl = dledAlbum.ArtworkUrl
                                             };
            }
        }
    }
}
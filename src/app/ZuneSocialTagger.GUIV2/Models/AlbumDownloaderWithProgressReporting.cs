using System;
using System.Diagnostics;
using System.Linq;
using Caliburn.PresentationFramework;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumDownloaderWithProgressReporting
    {
        private readonly BindableCollection<Album> _albums;
        private int _downloadCounter;

        public event Action FinishedDownloadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };

        public AlbumDownloaderWithProgressReporting(BindableCollection<Album> albums)
        {
            _albums = albums;
        }

        public void Start()
        {
            for (int index = 0; index < _albums.Count; index++)
            {
                var album = _albums[index];
                string fullUrlToAlbumXmlDetails =
                    String.Concat("http://catalog.zune.net/v3.0/en-US/music/album/",
                                  album.ZuneAlbumMetaData.AlbumMediaId);

                //albums that have a valid url but have not been checked for validity yet are deemed unknown
                if (album.LinkStatus == LinkStatus.Unknown)
                {
                    var reader = new AlbumDetailsDownloader(fullUrlToAlbumXmlDetails);
                    int index1 = index;
                    reader.DownloadCompleted += dledAlbum =>
                                                    {
                                                        _downloadCounter++;
                                                        //Debug.WriteLine(_downloadCounter);

                                                        DownloadAlbum(dledAlbum, album);


                                                        var totalUnlinkedCount = _albums.Where(x=> x.LinkStatus != LinkStatus.Unlinked).Count();

                                                        this.ProgressChanged.Invoke(_downloadCounter,totalUnlinkedCount);

                                                        if (_downloadCounter == totalUnlinkedCount)
                                                        {
                                                            this.FinishedDownloadingAlbums.Invoke();
                                                        }
                                                    };
                    reader.Download();
                }
            }
        }

        public static void DownloadAlbum(AlbumMetaData dledAlbum, Album album)
        {
            if (dledAlbum == null || album.LinkStatus == LinkStatus.Unlinked)
            {
                Debug.WriteLine("could not get album :(");
                album.LinkStatus = LinkStatus.Unlinked;
            }
            else
            {
                bool artistTitlesMatch = dledAlbum.AlbumArtist.ToUpper() ==
                                         album.ZuneAlbumMetaData.
                                             AlbumArtist.ToUpper();

                bool albumTitlesMatch = dledAlbum.AlbumTitle.ToUpper() ==
                                        album.ZuneAlbumMetaData.AlbumTitle.
                                            ToUpper();


                //TODO: find better way to do comparison
                string firstTwoChars =
                    new string(
                        album.ZuneAlbumMetaData.AlbumTitle.ToUpper().Take(2)
                            .ToArray());
                bool albumTitlesFirstTwoCharactersMatch =
                    dledAlbum.AlbumTitle.ToUpper().StartsWith(firstTwoChars);

                if (!artistTitlesMatch)
                    album.LinkStatus = LinkStatus.AlbumOrArtistMismatch;

                if (!artistTitlesMatch && !albumTitlesMatch)
                    album.LinkStatus = LinkStatus.AlbumOrArtistMismatch;

                if (albumTitlesMatch && artistTitlesMatch)
                    album.LinkStatus = LinkStatus.Linked;

                if (artistTitlesMatch && !albumTitlesMatch)
                    album.LinkStatus = LinkStatus.Linked;

                if (!albumTitlesFirstTwoCharactersMatch)
                    album.LinkStatus = LinkStatus.AlbumOrArtistMismatch;


                //if first or second character of album title is different...
            }

            album.WebAlbumMetaData = dledAlbum;
        }
    }
}
    

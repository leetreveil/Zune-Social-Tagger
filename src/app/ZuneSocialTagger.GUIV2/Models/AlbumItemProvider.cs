using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumItemProvider : IItemsProvider<Album>
    {
        private readonly List<Album> _collectionSource;
        private int _downloadCounter;

        public event Action AllDownloadsComplete = delegate { };
        public event Action ItemFinishedDownloading = delegate { };

        public AlbumItemProvider(List<Album> collectionSource)
        {
            _collectionSource = collectionSource;
        }

        public int FetchCount()
        {
            return _collectionSource.Count;
        }

        public IList<Album> FetchRange(int startIndex, int count)
        {
            Trace.WriteLine("FetchRange: " + startIndex + "," + count);

            List<Album> list = new List<Album>();

            for (int i = startIndex; i < startIndex + count; i++)
            {
                Album album = startIndex < _collectionSource.Count
                                  ?
                                      _collectionSource[i]
                                  : _collectionSource[i - count];


                string fullUrlToAlbumXmlDetails =
                    String.Concat("http://catalog.zune.net/v3.0/en-US/music/album/",
                                  album.ZuneAlbumMetaData.AlbumMediaId);

                //albums that have a valid url but have not been checked for validity yet are deemed unknown
                if (album.IsLinked == LinkStatus.Unknown)
                {
                    var reader = new AlbumDetailsDownloader(fullUrlToAlbumXmlDetails);
                    reader.DownloadCompleted += dledAlbum =>
                        {
                            _downloadCounter++;
                            Debug.WriteLine(_downloadCounter);

                            if (dledAlbum == null)
                            {
                                Debug.WriteLine("could not get album :(");
                                album.IsLinked = LinkStatus.Unlinked;
                            }
                            else
                            {
                                bool artistTitlesMatch = dledAlbum.AlbumArtist.ToUpper() == 
                                                       album.ZuneAlbumMetaData.AlbumArtist.ToUpper();

                                bool albumTitlesMatch = dledAlbum.AlbumTitle.ToUpper() ==
                                                       album.ZuneAlbumMetaData.AlbumTitle.ToUpper();



                                //TODO: find better way to do comparison
                                string firstTwoChars = new string(album.ZuneAlbumMetaData.AlbumTitle.ToUpper().Take(2).ToArray());
                                bool albumTitlesFirstTwoCharactersMatch = dledAlbum.AlbumTitle.ToUpper().StartsWith(firstTwoChars);

                                if (!artistTitlesMatch) album.IsLinked = LinkStatus.AlbumOrArtistMismatch;

                                if (!artistTitlesMatch && !albumTitlesMatch)  album.IsLinked = LinkStatus.AlbumOrArtistMismatch;

                                if (albumTitlesMatch && artistTitlesMatch) album.IsLinked = LinkStatus.Linked;

                                if (artistTitlesMatch && !albumTitlesMatch) album.IsLinked = LinkStatus.Linked;

                                if (!albumTitlesFirstTwoCharactersMatch)
                                    album.IsLinked = LinkStatus.AlbumOrArtistMismatch;
                 

                                //if first or second character of album title is different...
                            }

                            album.WebAlbumMetaData = dledAlbum;

                            int count1 = _collectionSource.Where(x => x.IsLinked != LinkStatus.Unlinked).Count();
                            Debug.WriteLine("current total count: " + count1);

                            if (_downloadCounter == count1)
                                this.AllDownloadsComplete.Invoke();

                            this.ItemFinishedDownloading.Invoke();
                        };
                    reader.Start();


                }

                list.Add(album);


            }

            return list;
        }
    }
}
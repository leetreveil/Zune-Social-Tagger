using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumItemProvider : IItemsProvider<Album>
    {
        private readonly List<Album> _collectionSource;

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
                                  album.AlbumMediaId);


                //do not attempt to load albums that are not linked
                if (album.IsLinked)
                {
                    var reader = new AlbumDocumentReader(fullUrlToAlbumXmlDetails);
                    reader.DownloadCompleted += dledAlbum =>
                                                    {
                                                        if (dledAlbum == null)
                                                        {
                                                            Debug.WriteLine("could not get album :(");
                                                            album.IsLinked = false;
                                                        }

                                                        album.WebAlbumMetaData = dledAlbum;
                                                    };
                    reader.Start();
                }

                list.Add(album);
            }

            return list;
        }
    }
}
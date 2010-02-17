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
                Album album;

                if (startIndex < _collectionSource.Count)
                {
                    album = _collectionSource[i];
                }
                else
                {
                    album = _collectionSource[i - count];
                }


                //if (startIndex >= count -10)
                //{
                //    album = _collectionSource[i -1];
                //}

                string fullUrlToAlbumXmlDetails =
                    String.Concat("http://catalog.zune.net/v3.0/en-US/music/album/",
                                     album.AlbumMediaId);


                //TODO: figure out how to only download images /album when the view displays them, i.e. a virtual view

                //do not attempt to load albums that are not linked
                if (album.IsLinked)
                {
                    try
                    {
                        var reader = new AlbumDocumentReader();

                        if (reader.Initialize(fullUrlToAlbumXmlDetails))
                        {
                            reader.DownloadCompleted +=
                                dledAlbum =>
                                    {
                                        album.WebAlbumMetaData = dledAlbum;
                                    };
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Could not get album details");
                    }
                }

                list.Add(album);
            }

            return list;
        }
    }
}
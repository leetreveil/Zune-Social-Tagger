using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MicrosoftZuneInterop;
using MicrosoftZuneLibrary;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public class ZuneDatabaseReader : IZuneDatabaseReader
    {
        private ZuneLibrary _zuneLibrary;

        public event Action FinishedReadingAlbums = delegate { };
        public event Action<int, int> ProgressChanged = delegate { };
        public event Action StartedReadingAlbums = delegate { };

        public bool Initialize()
        {
            //Just copying what the zune software does internally here to initialize the database
            _zuneLibrary = new ZuneLibrary();

            bool dbRebult;

            //anything other than 0 means an error occured reading the database
            int num = _zuneLibrary.Initialize(null, out dbRebult);

            if (num > -1)
            {
                int phase2;
                _zuneLibrary.Phase2Initialization(out phase2);
                _zuneLibrary.CleanupTransientMedia();
            }
            else
            {
                return false;
            }

            return true;
        }

        private static T GetFieldValue<T>(int mediaId, EListType listType, int atom, T defaultValue)
        {
            int[] columnIndexes = new int[] {atom};
            object[] fieldValues = new object[] {defaultValue};
            ZuneLibrary.GetFieldValues(mediaId, listType, 1, columnIndexes, fieldValues,
                                       new QueryPropertyBag());

            return (T) fieldValues[0];
        }

        private ZuneQueryList GetAlbumQueryList()
        {
            return _zuneLibrary.QueryDatabase(EQueryType.eQueryTypeAllAlbums, 0,
                                              EQuerySortType.eQuerySortOrderAscending,
                                              (uint)SchemaMap.kiIndex_AlbumID, new QueryPropertyBag());
        }

        private ZuneQueryList GetAlbumQueryListSorted(uint sortAtom)
        {
            var sortType = (sortAtom == (uint) SchemaMap.kiIndex_DateAdded)
                               ? EQuerySortType.eQuerySortOrderDescending
                               : EQuerySortType.eQuerySortOrderAscending;


            return _zuneLibrary.QueryDatabase(EQueryType.eQueryTypeAllAlbums, 0, 
                sortType, sortAtom, new QueryPropertyBag());
        }

        public IEnumerable<DbAlbum> ReadAlbums(SortOrder sortOrder)
        {
            this.StartedReadingAlbums.Invoke();

            uint sortAtom = 0;
            switch (sortOrder)
            {
                case SortOrder.DateAdded:
                    sortAtom = (uint) SchemaMap.kiIndex_DateAdded;
                    break;
                case SortOrder.Album:
                    sortAtom = (uint)SchemaMap.kiIndex_Title;
                    break;
                case SortOrder.Artist:
                    sortAtom = (uint)SchemaMap.kiIndex_DisplayArtist;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sortOrder");
            }

            //querying all albums, creates a property bag inside this method to query the database
            ZuneQueryList albums = GetAlbumQueryListSorted(sortAtom);
            albums.AddRef();

            var uniqueIds = albums.GetUniqueIds();

            for (int i = 0; i < uniqueIds.Count; i++)
            {
                yield return GetAlbumMin((int)uniqueIds[i]);
                ProgressChanged.Invoke(i, uniqueIds.Count - 1);
            }

            FinishedReadingAlbums.Invoke();

            albums.Release();
            albums.Dispose();
        }

        public DbAlbum GetAlbum(int index)
        {
            AlbumMetadata albumMetadata = _zuneLibrary.GetAlbumMetadata(index);

            var albumMediaId = GetFieldValue(index, EListType.eAlbumList,
                                                ZuneQueryList.AtomNameToAtom("ZuneMediaID"), Guid.Empty);

            var dateAdded = GetFieldValue(index, EListType.eAlbumList,
                                            ZuneQueryList.AtomNameToAtom("DateAdded"), new DateTime());

            var album = new DbAlbum
            {
                AlbumMediaId = albumMediaId,
                DateAdded = dateAdded,
                Title = albumMetadata.AlbumTitle,
                Artist = albumMetadata.AlbumArtist,
                ArtworkUrl = albumMetadata.CoverUrl,
                MediaId = albumMetadata.MediaId,
                ReleaseYear = albumMetadata.ReleaseYear.ToString(),
                TrackCount = (int)albumMetadata.TrackCount,
                Tracks = GetTracksForAlbum(albumMetadata.MediaId).ToList()
            };


            albumMetadata.Dispose();

            return album;
        }

        public DbAlbum GetAlbumMin(int index)
        {
            AlbumMetadata albumMetadata = _zuneLibrary.GetAlbumMetadata(index);

            var albumMediaId = GetFieldValue(index, EListType.eAlbumList,
                                 ZuneQueryList.AtomNameToAtom("ZuneMediaID"), Guid.Empty);

            var dateAdded = GetFieldValue(index, EListType.eAlbumList,
                                          ZuneQueryList.AtomNameToAtom("DateAdded"), new DateTime());

            var album = new DbAlbum
            {
                AlbumMediaId = albumMediaId,
                DateAdded = dateAdded,
                Title = albumMetadata.AlbumTitle,
                Artist = albumMetadata.AlbumArtist,
                ArtworkUrl = albumMetadata.CoverUrl,
                MediaId = albumMetadata.MediaId,
            };

            albumMetadata.Dispose();

            return album;
        }

        public IEnumerable<DbTrack> GetTracksForAlbum(int albumId)
        {
            ZuneQueryList zuneQueryList = _zuneLibrary.GetTracksByAlbum(0, albumId,
                                                                        EQuerySortType.eQuerySortOrderAscending,
                                                                        (uint) SchemaMap.kiIndex_AlbumID);

            for (int i = 0; i < zuneQueryList.Count; i++)
            {
                var track = new ZuneQueryItem(zuneQueryList, i);

                yield return new DbTrack
                 {
                     FilePath = GetTrackValue<string>(track, "SourceURL"),
                     MediaId = GetTrackValue<Guid>(track, "ZuneMediaID"),
                     Title = GetTrackValue<string>(track, "Title"),
                     TrackNumber = GetTrackValue<long>(track, "WM/TrackNumber").ToString()
                 };
            }

            zuneQueryList.Dispose();
        }

        private static T GetTrackValue<T>(ZuneQueryItem item, string fieldName)
        {
            var result = item.GetFieldValue(typeof (T), (uint) ZuneQueryList.AtomNameToAtom(fieldName));
            if (result != null)
            {
                return (T) result;
            }

            return default(T);
        }

        public void RemoveAlbumFromDatabase(int albumId)
        {
            ZuneQueryList zuneQueryList = _zuneLibrary.GetTracksByAlbum(0, albumId,
                                                                        EQuerySortType.eQuerySortOrderAscending,
                                                                        (uint) SchemaMap.kiIndex_AlbumID);
            for (int i = 0; i < zuneQueryList.Count; i++)
            {
                var track = new ZuneQueryItem(zuneQueryList, i);
                //TODO: see if we can delete the actual album
                _zuneLibrary.DeleteMedia(new[] {track.ID}, EMediaTypes.eMediaTypeAudio, false);
            }

            _zuneLibrary.CleanupTransientMedia();
            zuneQueryList.Dispose();
        }

        public void Dispose()
        {
            _zuneLibrary.Dispose();
        }
    }
}
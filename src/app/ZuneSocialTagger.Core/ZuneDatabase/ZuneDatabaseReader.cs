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
            try
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
            catch
            {
                return false;
            }
        }

        private static T GetFieldValue<T>(int mediaId, EListType listType, int atom, T defaultValue)
        {
            int[] columnIndexes = new int[] {atom};
            object[] fieldValues = new object[] {defaultValue};
            ZuneLibrary.GetFieldValues(mediaId, listType, 1, columnIndexes, fieldValues,
                                       new QueryPropertyBag());

            return (T) fieldValues[0];
        }

        private static T SetFieldValue<T>(int mediaId, EListType listType, int atom, T newValue)
        {
            int[] columnIndexes = new int[] { atom };
            object[] fieldValues = new object[] { newValue };
            ZuneLibrary.SetFieldValues(mediaId, listType, 1, columnIndexes, fieldValues, new QueryPropertyBag());
            return (T)fieldValues[0];
        }

        private ZuneQueryList GetAlbumQueryList()
        {
            return _zuneLibrary.QueryDatabase(EQueryType.eQueryTypeAllAlbums, 0,
                                              EQuerySortType.eQuerySortOrderAscending,
                                              (uint)SchemaMap.kiIndex_AlbumID, new QueryPropertyBag());
        }


        public IEnumerable<DbAlbum> ReadAlbums()
        {
            this.StartedReadingAlbums.Invoke();

            //querying all albums, creates a property bag inside this method to query the database
            ZuneQueryList albums = GetAlbumQueryList();
            albums.AddRef();

            var uniqueIds = albums.GetUniqueIds();

            for (int i = 0; i < uniqueIds.Count; i++)
            {
                object uniqueId = uniqueIds[i];
                yield return GetAlbum((int) uniqueId);
                ProgressChanged.Invoke(i, uniqueIds.Count -1);
            }

            FinishedReadingAlbums.Invoke();

            albums.Release();
            albums.Dispose();
        }

        public IEnumerable<DbAlbum> GetNewAlbums(IEnumerable<int> albumIds)
        {
            ZuneQueryList albums = GetAlbumQueryList();

            int[] uniqueIds = (int[])albums.GetUniqueIds().ToArray(typeof(int));


            //get all album ids that are in the zune db but NOT in the id list
            IEnumerable<int> newIds = uniqueIds.Except(albumIds);

            return newIds.Select(GetAlbum);
        }

        public IEnumerable<int> GetRemovedAlbums(IEnumerable<int> albumIds)
        {
            ZuneQueryList albums = GetAlbumQueryList();

            //get a list of album ids
            int[] uniqueIds = (int[])albums.GetUniqueIds().ToArray(typeof(int));

            return albumIds.Except(uniqueIds);
        }

        public DbAlbum GetAlbumByAlbumTitle(string albumTitle)
        {
            ////querying all albums, creates a property bag inside this method to query the database
            ////thats why we can pass null for the propertybag
            //ZuneQueryList albums = _zuneLibrary.QueryDatabase(EQueryType.eQueryTypeAllAlbums, 0,
            //                                                  EQuerySortType.eQuerySortOrderAscending,
            //                                                  (uint) SchemaMap.kiIndex_AlbumID, null);

            //int searchForString = albums.SearchForString((uint) SchemaMap.kiIndex_WMAlbumArtist, false, albumTitle);


            //var album = GetAlbum(searchForString);

            return null;
        }

        public bool DoesAlbumExist(int index)
        {
            try
            {
                var albumMetadata = _zuneLibrary.GetAlbumMetadata(index);

                return albumMetadata != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DbAlbum GetAlbum(int index)
        {
            AlbumMetadata albumMetadata = _zuneLibrary.GetAlbumMetadata(index);

            var myNewGuid = Guid.NewGuid();

            var result = SetFieldValue(index, EListType.eAlbumList,
                          ZuneQueryList.AtomNameToAtom("ZuneMediaID"), myNewGuid);

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
                                TrackCount = (int) albumMetadata.TrackCount,
                                Tracks = GetTracksForAlbum(albumMetadata.MediaId).ToList()
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

                //for (int j = 0; j < 2000; j++)
                //{
                //    try
                //    {
                //        var test = track.GetFieldValue(typeof(long), (uint)j);

                //        if (test != null)
                //        {
                //            Trace.WriteLine(ZuneQueryList.AtomToAtomName(j));
                //            Trace.WriteLine(test);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.WriteLine("FAILED ON");
                //    }
                //}

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
            try
            {
                return (T) item.GetFieldValue(typeof (T), (uint) ZuneQueryList.AtomNameToAtom(fieldName));
            }
            catch (Exception)
            {
                return default(T);
            }
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

        public void AddTrackToDatabase(string filePath)
        {
            _zuneLibrary.AddMedia(filePath);
        }

        public bool CanInitialize
        {
            get
            {
                //The version that this program was written to support, in future versions methods could change
                //so updates will probably be needed

                if (!File.Exists("ZuneDBApi.dll"))
                    throw new FileNotFoundException(
                        "Could not find ZuneDBApi.dll. Are you sure Zune Social Tagger is installed in the Zune application folder?");
                return true;
            }
        }

        public void Dispose()
        {
            _zuneLibrary.Dispose();
        }
    }
}
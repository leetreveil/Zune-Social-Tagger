using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public interface IZuneDatabaseReader : IDisposable
    {
        bool Initialize();
        IEnumerable<DbAlbum> ReadAlbums();
        DbAlbum GetAlbum(int index);
        IEnumerable<DbTrack> GetTracksForAlbum(int albumId);
        event Action StartedReadingAlbums;
        event Action FinishedReadingAlbums;
        event Action<int, int> ProgressChanged;

        /// <summary>
        /// This could potentially take a long time because were effectively refreshing the database
        /// </summary>
        /// <param name="albumTitle"></param>
        /// <returns></returns>
        DbAlbum GetAlbumByAlbumTitle(string albumTitle);

        bool DoesAlbumExist(int index);
        void RemoveAlbumFromDatabase(int albumId );
        void AddTrackToDatabase(string filePath);

        /// <summary>
        /// Put file system depedency checks in here
        /// </summary>
        /// <returns></returns>
        bool CanInitialize { get; }

        IEnumerable<DbAlbum> GetNewAlbums(IEnumerable<int> albumIds);
        IEnumerable<int> GetRemovedAlbums(IEnumerable<int> albumIds);
    }
}
using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public interface IZuneDatabaseReader : IDisposable
    {
        bool Initialize();
        IEnumerable<DbAlbum> ReadAlbums(SortOrder sortOrder);
        DbAlbum GetAlbum(int index);
        IEnumerable<DbTrack> GetTracksForAlbum(int albumId);
        event Action StartedReadingAlbums;
        event Action FinishedReadingAlbums;
        event Action<int, int> ProgressChanged;

        void RemoveAlbumFromDatabase(int albumId );
        void AddTrackToDatabase(string filePath);
    }
}
using System;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneDatabase;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneDbAdapter : IDisposable
    {
        bool CanInitialize { get; }
        bool Initialize();
        IEnumerable<AlbumDetails> ReadAlbums();
        AlbumDetails GetAlbum(int index);
        IEnumerable<Track> GetTracksForAlbum(int albumId);
        event Action FinishedReadingAlbums;
        event Action<int, int> ProgressChanged;
        bool DoesAlbumExist(int index);
    }
}
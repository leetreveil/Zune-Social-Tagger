using System;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneDbAdapter : IDisposable
    {
        bool CanInitialize { get; }
        bool Initialize();
        IEnumerable<AlbumDetailsViewModel> ReadAlbums();
        AlbumDetailsViewModel GetAlbum(int index);
        IEnumerable<Track> GetTracksForAlbum(int albumId);
        event Action FinishedReadingAlbums;
        event Action<int, int> ProgressChanged;
        bool DoesAlbumExist(int index);
    }
}
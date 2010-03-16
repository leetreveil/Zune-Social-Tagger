using System;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneDbAdapter : IDisposable
    {
        bool Initialize();
        IEnumerable<AlbumDetailsViewModel> ReadAlbums();
        AlbumDetailsViewModel GetAlbum(int index);
        IEnumerable<Track> GetTracksForAlbum(int albumId);
        event Action FinishedReadingAlbums;
        event Action<int, int> ProgressChanged;

        ///// <summary>
        ///// This could potentially take a long time because were effectively refreshing the database
        ///// </summary>
        ///// <param name="albumTitle"></param>
        ///// <returns></returns>
        //AlbumDetailsViewModel GetAlbumByAlbumTitle(string albumTitle);

        bool DoesAlbumExist(int index);
        //void RemoveAlbumFromDatabase(int albumId );
        //void AddTrackToDatabase(string filePath);
    }
}
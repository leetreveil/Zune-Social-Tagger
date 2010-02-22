using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public interface IZuneDatabaseReader
    {
        bool Initialize();
        IEnumerable<Album> ReadAlbums();
        Album GetAlbum(int index);
        IEnumerable<Track> GetTracksForAlbum(int index);
    }
}
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public interface IZuneDatabaseReader
    {
        bool Initialize();
        IEnumerable<DbAlbumDetails> ReadAlbums();
        DbAlbumDetails GetAlbum(int index);
    }
}
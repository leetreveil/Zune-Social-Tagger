using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    public interface IZuneDatabaseReader
    {
        void Load();
        IEnumerable<DbAlbumDetails> ReadAlbums();
        DbAlbumDetails GetAlbum(int index);
    }
}
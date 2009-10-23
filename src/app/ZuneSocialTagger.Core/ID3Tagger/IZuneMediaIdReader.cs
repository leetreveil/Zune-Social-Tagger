using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public interface IZuneMediaIdReader
    {
        IEnumerable<MediaIdGuid> ReadMediaIds();
    }
}
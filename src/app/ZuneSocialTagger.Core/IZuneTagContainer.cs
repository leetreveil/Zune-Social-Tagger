using System.Collections.Generic;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.Core
{
    public interface IZuneTagContainer
    {
        IEnumerable<MediaIdGuid> ReadMediaIds();
        void Add(MediaIdGuid mediaIDGuid);
        MetaData ReadMetaData();
        void WriteMetaData(MetaData metaData);
    }
}
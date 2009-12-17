using System.Collections.Generic;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.Core
{
    public interface IZuneTagContainer
    {
        IEnumerable<MediaIdGuid> ReadMediaIds();
        void Add(MediaIdGuid mediaIDGuid);
        Track ReadMetaData();
        void WriteMetaData(Track metaData);
    }
}
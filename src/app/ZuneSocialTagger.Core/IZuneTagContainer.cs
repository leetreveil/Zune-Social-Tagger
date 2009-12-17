using System.Collections.Generic;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.Core
{
    public interface IZuneTagContainer
    {
        IEnumerable<MediaIdGuid> ReadMediaIds();
        void AddZuneMediaId(MediaIdGuid mediaIDGuid);
        MetaData ReadMetaData();
        void AddMetaData(MetaData metaData);
    }
}
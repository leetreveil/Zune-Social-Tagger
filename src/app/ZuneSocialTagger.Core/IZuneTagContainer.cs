using System.Collections.Generic;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.Core
{
    public interface IZuneTagContainer
    {
        IEnumerable<MediaIdGuid> ReadMediaIds();
        void AddZuneMediaId(MediaIdGuid mediaIDGuid);
        MetaData ReadMetaData();
        void AddMetaData(MetaData metaData);
        void WriteToFile(string filePath);
    }
}
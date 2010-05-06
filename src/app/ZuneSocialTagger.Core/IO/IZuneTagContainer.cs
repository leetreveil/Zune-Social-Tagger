using System.Collections.Generic;

namespace ZuneSocialTagger.Core.IO
{
    public interface IZuneTagContainer
    {
        IEnumerable<ZuneAttribute> ReadZuneAttributes();
        void AddZuneAttribute(ZuneAttribute zuneAttribute);
        MetaData ReadMetaData();
        void AddMetaData(MetaData metaData);
        void WriteToFile(string filePath);
        void RemoveZuneAttribute(string name);
    }
}
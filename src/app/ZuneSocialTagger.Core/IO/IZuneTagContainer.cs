using System.Collections.Generic;

namespace ZuneSocialTagger.Core.IO
{
    public interface IZuneTagContainer
    {
        void AddZuneAttribute(ZuneAttribute zuneAttribute);
        void AddMetaData(MetaData metaData);
        void WriteToFile();
        void RemoveZuneAttribute(string name);
        MetaData MetaData { get; }
        IEnumerable<ZuneAttribute> ZuneAttributes { get; }
    }
}
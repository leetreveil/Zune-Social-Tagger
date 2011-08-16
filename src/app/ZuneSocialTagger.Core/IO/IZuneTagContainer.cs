using System;

namespace ZuneSocialTagger.Core.IO
{
    public interface IZuneTagContainer : IDisposable
    {
        void AddZuneAttribute(ZuneAttribute zuneAttribute);
        void UpdateMetaData(MetaData metaData);
        void WriteToFile();
        void RemoveZuneAttribute(string name);
        MetaData MetaData { get; }
    }
}
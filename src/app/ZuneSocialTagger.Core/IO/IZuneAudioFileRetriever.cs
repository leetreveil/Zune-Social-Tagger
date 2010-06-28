using System.Collections.Generic;

namespace ZuneSocialTagger.Core.IO
{
    public interface IZuneAudioFileRetriever
    {
        List<IZuneTagContainer> Containers { get; }
        void GetContainers(IEnumerable<string> filePaths);
        void SortByTrackNumber();
    }
}
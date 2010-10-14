using System.Collections.Generic;

namespace ZuneSocialTagger.Core.IO
{
    public interface IZuneAudioFileRetriever
    {
        IList<IZuneTagContainer> GetContainers(IEnumerable<string> filePaths);
    }
}
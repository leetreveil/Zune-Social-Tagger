using System;
using System.Collections.Generic;
using System.Linq;

namespace ZuneSocialTagger.Core.IO
{
    public class ZuneAudioFileRetriever : IZuneAudioFileRetriever
    {
        public IList<IZuneTagContainer> GetContainers(IEnumerable<string> filePaths)
        {
            return (from filePath in filePaths
                               let container = ZuneTagContainerFactory.GetContainer(filePath)
                               select container).ToList();
        }

        public static IList<IZuneTagContainer> SortByTrackNumber(IList<IZuneTagContainer> containers)
        {
            return containers.OrderBy(SortByTrackNumberImpl()).ToList();
        }

        private static Func<IZuneTagContainer, int> SortByTrackNumberImpl()
        {
            return arg =>
            {
                int result;
                Int32.TryParse(arg.MetaData.TrackNumber, out result);

                return result;
            };
        }
    }
}
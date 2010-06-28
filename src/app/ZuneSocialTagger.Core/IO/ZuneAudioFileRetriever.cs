using System;
using System.Collections.Generic;
using System.Linq;

namespace ZuneSocialTagger.Core.IO
{
    public class ZuneAudioFileRetriever : IZuneAudioFileRetriever
    {
        public ZuneAudioFileRetriever()
        {
            this.Containers = new List<IZuneTagContainer>();
        }
        public List<IZuneTagContainer> Containers { get; private set; }

        public void GetContainers(IEnumerable<string> filePaths)
        {
            this.Containers = (from filePath in filePaths
                               let container = ZuneTagContainerFactory.GetContainer(filePath)
                               select container).ToList();
        }

        public void SortByTrackNumber()
        {
            this.Containers = this.Containers.OrderBy(SortByTrackNumberImpl()).ToList();
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
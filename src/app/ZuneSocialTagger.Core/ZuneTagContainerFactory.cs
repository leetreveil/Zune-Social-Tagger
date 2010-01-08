using ASFTag.Net;
using ID3Tag;
using System.IO;
using ZuneSocialTagger.Core.ID3Tagger;
using ZuneSocialTagger.Core.WMATagger;

namespace ZuneSocialTagger.Core
{
    public static class ZuneTagContainerFactory
    {
        public static IZuneTagContainer GetContainer(string path)
        {
            string extension = Path.GetExtension(path);

            if (extension == ".mp3")
            {
                FileState status = Id3TagManager.GetTagsStatus(path);

                //if we just have id3v1.1 tags
                if (status.Id3V1TagFound && !status.Id3V2TagFound)
                    throw new ID3TagException("cannot read id3v1.1");

                return new ZuneMP3TagContainer(Id3TagManager.ReadV2Tag(path));
            }

            if (extension == ".wma")
            {
                var container = new ZuneWMATagContainer();

                TagContainer fileContainer = ASFTagManager.ReadTag(path);

                container.AddRange(fileContainer);

                return container;
            }

            return null;
        }
    }
}
using ASFTag;
using Id3Tag;
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

            if (extension.ToLower() == ".mp3")
            {
                var tagManager = new Id3TagManager();


                //TODO: app crashes here when a file is loaded from a remote directory, i.e. on network
                FileState status = tagManager.GetTagsStatus(path);

                //if we just have id3v1.1 tags
                if (status.Id3V1TagFound && !status.Id3V2TagFound)
                    throw new Id3TagException("cannot read id3v1.1");

                return new ZuneMP3TagContainer(tagManager.ReadV2Tag(path));
            }

            if (extension.ToLower() == ".wma")
                return new ZuneWMATagContainer(ASFTagManager.ReadTag(path));

            return null;
        }
    }
}
using ID3Tag;
using ID3Tag.HighLevel;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public static class ZuneTagContainerFactory
    {
        public static ZuneTagContainer GetContainer(string path)
        {
            FileState status = Id3TagManager.GetTagsStatus(path);

            return status.Id3V2TagFound ? new ZuneTagContainer(Id3TagManager.ReadV2Tag(path)) : null;
        }
    }
}
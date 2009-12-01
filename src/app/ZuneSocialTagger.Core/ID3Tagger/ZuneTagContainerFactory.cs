using ID3Tag;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public static class ZuneTagContainerFactory
    {
        public static ZuneTagContainer GetContainer(string path)
        {
            FileState status = Id3TagManager.GetTagsStatus(path);

            //if we just have id3v1.1 tags
            if (status.Id3V1TagFound && !status.Id3V2TagFound)
                throw new ID3TagException("cannot read id3v1.1");

            return new ZuneTagContainer(Id3TagManager.ReadV2Tag(path));
        }
    }
}
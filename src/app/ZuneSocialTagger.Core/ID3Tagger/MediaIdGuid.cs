using System;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class MediaIdGuid
    {
        public string MediaId { get; set; }
        public Guid Guid { get; set; }

        public MediaIdGuid(string mediaID, Guid guid)
        {
            MediaId = mediaID;
            Guid = guid;
        }
    }
}
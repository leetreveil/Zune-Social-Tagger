using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ID3Tagger
{
    public class MediaIdGuidComparer : IEqualityComparer<MediaIdGuid>
    {
        public bool Equals(MediaIdGuid x, MediaIdGuid y)
        {
            return x.Guid == y.Guid && x.MediaId == y.MediaId;
        }

        public int GetHashCode(MediaIdGuid obj)
        {
            return obj.Guid.GetHashCode() + obj.MediaId.GetHashCode();
        }
    }
}
using System;

namespace ZuneSocialTagger.Core
{
    public class MediaIdGuid
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }

        public MediaIdGuid(string mediaID, Guid guid)
        {
            Name = mediaID;
            Guid = guid;
        }

        public override bool Equals(object obj)
        {
            var newGuid = (MediaIdGuid) obj;

            return this.Guid == newGuid.Guid;
        }

        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }

    }
}
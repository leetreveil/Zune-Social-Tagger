using System;

namespace ZuneSocialTagger.Core
{
    /// <summary>
    /// A zune attribute, Name = ZuneAlbumArtistId, Guid = 3ed50a00-0600-11db-89ca-0019b92a3933
    /// </summary>
    public class ZuneAttribute
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }

        public ZuneAttribute(string mediaID, Guid guid)
        {
            Name = mediaID;
            Guid = guid;
        }

        public override bool Equals(object obj)
        {
            var newGuid = (ZuneAttribute) obj;

            return this.Guid == newGuid.Guid;
        }

        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }
    }
}
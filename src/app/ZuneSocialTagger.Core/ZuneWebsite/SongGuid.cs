using System;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class SongGuid
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }

        public bool IsValid()
        {
            //A songGuid is valid if its guid is not empty and its title is not empty or null
            return Guid != Guid.Empty && !String.IsNullOrEmpty(Title);
        }
    }
}
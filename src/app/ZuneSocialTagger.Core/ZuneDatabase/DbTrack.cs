using System;

namespace ZuneSocialTagger.Core.ZuneDatabase
{
    [Serializable]
    public class DbTrack
    {
        public Guid MediaId { get; set; }
        public string FilePath { get; set; }
        public string Title { get; set; }
        public string TrackNumber { get; set; }
    }
}
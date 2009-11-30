using System;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearchResult
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public Guid Guid { get; set; }
        public string ArtworkUrl { get; set; }
        public int? ReleaseYear { get; set; }
    }
}
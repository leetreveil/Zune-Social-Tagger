using System;
using System.Collections.Generic;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumWebpageScrapeResult
    {
        public Guid AlbumMediaID { get; set; }
        public Guid AlbumArtistID { get; set; }
        public IEnumerable<SongGuid> SongTitlesAndMediaID { get; set; }

        public string AlbumArtist { get; set; }
        public string AlbumTitle { get; set; }
        public int? AlbumReleaseYear { get; set; }
        public string AlbumArtworkUrl { get; set; }

        public bool IsValid()
        {
            return AlbumMediaID != Guid.Empty && AlbumArtistID != Guid.Empty && SongTitlesAndMediaID.AreAllValid();
        }
    }
}
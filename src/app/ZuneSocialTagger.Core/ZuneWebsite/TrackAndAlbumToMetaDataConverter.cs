using System;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class TrackAndAlbumToMetaDataConverter
    {
        private readonly Album _album;
        private readonly Track _track;

        public TrackAndAlbumToMetaDataConverter(Album album, Track track)
        {
            _album = album;
            _track = track;
        }

        public bool CanConvert
        {
            get { return _album.HasAllMetaData && _track.HasAllMetaData; }
        }

        public MetaData Convert()
        {
            return new MetaData
                       {
                           AlbumArtist = _album.Artist,
                           AlbumTitle = _album.Title,
                           ContributingArtist =
                               CombinePrimaryArtistAndAnyAdditionalArtists(_track.Artist, _track.ContributingArtists),
                           DiscNumber = ConvertDiscNoToString(_track.DiscNumber),
                           Genre = _track.Genre,
                           SongTitle = _track.Title,
                           TrackNumber = _track.TrackNumber.ToString(),
                           Year = _album.ReleaseYear.ToString()
                       };
        }

        public string ConvertDiscNoToString(int? discNumber)
        {
            return String.Format("{0}/{0}", discNumber);
        }

        public string CombinePrimaryArtistAndAnyAdditionalArtists(string primaryArtists, IEnumerable<string> additional)
        {
            var joinedList = new List<string> {primaryArtists};
            joinedList.AddRange(additional);

            return String.Join("/", joinedList.ToArray());
        }
    }
}
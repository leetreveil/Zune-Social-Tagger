using System;
using System.Linq;
using Caliburn.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public static class ZuneTypeConverters
    {
        public static AlbumDetails ConvertDbAlbumToAlbumDetails(Core.ZuneDatabase.Album dbAlbumDetails)
        {
            return new AlbumDetails
            {
                MediaId = dbAlbumDetails.MediaId,
                AlbumTitle = dbAlbumDetails.AlbumTitle,
                AlbumArtist = dbAlbumDetails.AlbumArtist,
                ArtworkUrl = dbAlbumDetails.ArtworkUrl,
                AlbumMediaId = dbAlbumDetails.AlbumMediaId,
                DateAdded = dbAlbumDetails.DateAdded,
                ReleaseYear = dbAlbumDetails.ReleaseYear,
                TrackCount = dbAlbumDetails.TrackCount
            };
        }

        public static LinkStatus GetAlbumLinkStatus(string albumTitle, string albumArtist, AlbumDetails existingAlbum)
        {
            bool artistTitlesMatch = albumArtist.ToUpper() ==
                                     existingAlbum.AlbumArtist.ToUpper();

            bool albumTitlesMatch = albumTitle.ToUpper() ==
                                    existingAlbum.AlbumTitle.ToUpper();


            //if first or second character of album title is different...
            //TODO: find better way to do comparison
            string firstTwoChars =
                new string(existingAlbum.AlbumTitle.ToUpper().Take(2).ToArray());

            bool albumTitlesFirstTwoCharactersMatch =
                albumTitle.ToUpper().StartsWith(firstTwoChars);

            if (!albumTitlesFirstTwoCharactersMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (!artistTitlesMatch)
                return LinkStatus.AlbumOrArtistMismatch;

            if (albumTitlesMatch)
                return LinkStatus.Linked;


            return LinkStatus.Linked;
        }
    }
}
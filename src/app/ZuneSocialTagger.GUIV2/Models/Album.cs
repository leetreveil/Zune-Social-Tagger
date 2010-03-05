using System;
using System.Linq;
using Caliburn.Core;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class Album : PropertyChangedBase
    {
        private AlbumDetails _webAlbumMetaData;
        private AlbumDetails _zuneAlbumMetaData;
        private LinkStatus _linkStatus;

        public LinkStatus LinkStatus
        {
            get { return _linkStatus; }
            set
            {
                _linkStatus = value;
                NotifyOfPropertyChange(() => this.LinkStatus);
            }
        }

        public AlbumDetails ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                _zuneAlbumMetaData = value;
                NotifyOfPropertyChange(() => this.ZuneAlbumMetaData);
            }
        }

        public AlbumDetails WebAlbumMetaData
        {
            get { return _webAlbumMetaData; }
            set
            {
                _webAlbumMetaData = value;
                NotifyOfPropertyChange(() => this.WebAlbumMetaData);
            }
        }

        public static Album GetNewAlbum(Core.ZuneDatabase.Album dbAlbumDetails)
        {
            var newAlbum = new Album();

            newAlbum.ZuneAlbumMetaData =
                new AlbumDetails
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


            //set album to unlinked if it doesn't have a media id
            if (String.IsNullOrEmpty(newAlbum.ZuneAlbumMetaData.AlbumMediaId))
                newAlbum.LinkStatus = LinkStatus.Unlinked;

            return newAlbum;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    /// <summary>
    /// Downloads the album details from the zune album's xml document
    /// </summary>
    public static class AlbumDetailsDownloader
    {
        public static void DownloadAsync(string url, Action<WebAlbum> callback)
        {
            var request = WebRequest.Create(new Uri(url));
            request.BeginGetResponse(ar => 
            {
                try
                {
                    using (var response = request.EndGetResponse(ar))
                    {
                        var reader = XmlReader.Create(response.GetResponseStream());
                        callback.Invoke(GetAlbumDetails(reader));
                    }
                }
                catch (Exception)
                {   
                    callback.Invoke(null);
                }
            },new object());
        }

        private static WebAlbum GetAlbumDetails(XmlReader reader)
        {
            var feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                return new WebAlbum
                {
                    Title = feed.Title.Text,
                    Artist = feed.GetArtist(),
                    ArtworkUrl = feed.GetArtworkUrl(),
                    ReleaseYear = feed.GetReleaseYear(),
                    Tracks = GetTracks(feed).ToList(),
                    Genre = feed.GetGenre(),
                    AlbumMediaId = feed.Id.ExtractGuidFromUrnUuid()
                };
            }

            return null;
        }

        private static IEnumerable<WebTrack> GetTracks(SyndicationFeed feed)
        {
            return feed.Items.Select(item => new WebTrack
            {
                MediaId = item.Id.ExtractGuidFromUrnUuid(),
                ArtistMediaId = item.GetArtistMediaIdFromTrack(),
                AlbumMediaId = item.GetAlbumMediaIdFromTrack(),
                Title = item.Title.Text,
                DiscNumber = item.GetDiscNumber(),
                TrackNumber = item.GetTrackNumberFromTrack(),
                Genre = item.GetGenre(),
                ContributingArtists = item.GetContributingArtists().ToList(),
                Artist = item.GetArtist()
            }).ToList();
        }
    }
}



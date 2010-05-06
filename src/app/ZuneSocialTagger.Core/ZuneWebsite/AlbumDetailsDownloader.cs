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
    public class AlbumDetailsDownloader
    {
        private readonly string _url;
        private XmlReader _reader;
        private SyndicationFeed _feed;
        private readonly WebClient _client;

        public event Action<WebAlbum,DownloadState> DownloadCompleted = delegate { };

        public AlbumDetailsDownloader(string url)
        {
            _url = url;
            _client = new WebClient();
            _client.DownloadDataCompleted += ClientDownloadDataCompleted;
        }

        public void Cancel()
        {
            _client.CancelAsync();
        }

        public void DownloadAsync()
        {
            _client.DownloadDataAsync(new Uri(_url));
        }

        public WebAlbum Download()
        {
            byte[] downloadData = _client.DownloadData(new Uri(_url));

            _reader = XmlReader.Create(new MemoryStream(downloadData));

            return Read();
        }

        void ClientDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled)
                this.DownloadCompleted.Invoke(null, DownloadState.Cancelled);
            else
            {
                if (e.Error == null)
                {
                    try
                    {
                        _reader = XmlReader.Create(new MemoryStream(e.Result));

                        this.DownloadCompleted.Invoke(this.Read(), DownloadState.Success);
                    }
                    catch
                    {
                        this.DownloadCompleted.Invoke(null, DownloadState.Error);
                    }
                }
                else
                {
                    this.DownloadCompleted.Invoke(null, DownloadState.Error);
                }
            }
        }

        private WebAlbum Read()
        {
            _feed = SyndicationFeed.Load(_reader);

            return _feed != null ? GetAlbumDetails() : null;
        }

        private WebAlbum GetAlbumDetails()
        {
            return new WebAlbum
                       {
                           Title = _feed.Title.Text,
                           Artist = _feed.GetArtist(),
                           ArtworkUrl = _feed.GetArtworkUrl(),
                           ReleaseYear = _feed.GetReleaseYear(),
                           Tracks = GetTracks(),
                           AlbumMediaId = _feed.Id.ExtractGuidFromUrnUuid()
                       };
        }

        private List<WebTrack> GetTracks()
        {
            return _feed.Items.Select(item => new WebTrack
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



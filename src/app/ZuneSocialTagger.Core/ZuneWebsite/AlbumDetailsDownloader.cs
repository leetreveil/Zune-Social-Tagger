using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.ServiceModel.Syndication;
using System.IO;

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

        public event Action<Album,DownloadState> DownloadCompleted = delegate { };

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

        public Album Download()
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

        private Album Read()
        {
            _feed = SyndicationFeed.Load(_reader);

            return _feed != null ? GetAlbumDetails() : null;
        }

        private Album GetAlbumDetails()
        {
            return new Album
                       {
                           Title = _feed.Title.Text,
                           Artist = _feed.GetArtist(),
                           ArtworkUrl = _feed.GetArtworkUrl(),
                           ReleaseYear = _feed.GetReleaseYear(),
                           Tracks = GetTracks(),
                           AlbumMediaId = _feed.Id.ExtractGuidFromUrnUuid()
                       };
        }

        private List<Track> GetTracks()
        {
            return _feed.Items.Select(item => new Track
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



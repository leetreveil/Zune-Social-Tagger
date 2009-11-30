using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Threading;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumDocumentReader
    {
        private readonly XmlReader _reader;

        public AlbumDocumentReader(string url) : this(XmlReader.Create(url))
        {

        }

        public AlbumDocumentReader(XmlReader reader)
        {
            _reader = reader;
        }

        public void ReadAsync(Action<Album> callback)
        {
            //TODO: may need to wrap the full class to be async because the xml reader could be taking time too
            ThreadPool.QueueUserWorkItem(_ => callback(Read()));
        }

        public Album Read()
        {
            var album = new Album();
            SyndicationFeed feed = SyndicationFeed.Load(_reader);

            if (feed != null)
            {
                album.AlbumTitle = feed.Title.Text;
                album.AlbumArtist = GetAlbumArtist(feed);
                album.AlbumMediaID = feed.Id.ExtractGuidFromUrnUuid();
                album.AlbumReleaseYear = GetReleaseYear(feed);
                album.AlbumArtworkUrl = GetArtworkUrl(feed);
                album.Tracks = GetTracks(feed.Items);
            }

            return album;
        }

        private IEnumerable<Track> GetTracks(IEnumerable<SyndicationItem> items)
        {
            foreach (var item in items)
            {
                yield return new Track{Title = item.Title.Text,
                                       MediaID = item.Id.ExtractGuidFromUrnUuid(),
                                       Artist = GetArtistFromTrack(item),
                                       ArtistMediaID = GetArtistMediaIDFromTrack(item),
                                       Number = GetTrackNumberFromTrack(item)
                };
            }
        }

        private string GetArtistFromTrack(SyndicationItem item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");


            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        private Guid GetArtistMediaIDFromTrack(SyndicationItem item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");

            return primaryArtistElement != null ?  primaryArtistElement.Elements().First().Value.ExtractGuidFromUrnUuid() : new Guid();
        }

        private int? GetTrackNumberFromTrack(SyndicationItem item)
        {
            XElement trackNumberElement = GetElement(item, "trackNumber");

            return trackNumberElement != null ? int.Parse(trackNumberElement.Value) : (int?) null;
        }

        private string GetAlbumArtist(SyndicationFeed feed)
        {
            XElement primaryArtistElement = GetElement(feed, "primaryArtist");

            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        private int? GetReleaseYear(SyndicationFeed feed)
        {
            XElement releaseDateElement = GetElement(feed, "releaseDate");

            return releaseDateElement != null ? DateTime.Parse(releaseDateElement.Value).Year : (int?) null;
        }

        private string GetArtworkUrl(SyndicationFeed feed)
        {
            XElement imageElement = GetElement(feed, "image");

            return imageElement != null ? String.Format("http://image.catalog.zune.net/v3.0/image/{0}?width=234&height=320",
                imageElement.Elements().First().Value.ExtractGuidFromUrnUuid()) : null;
        }

        private XElement GetElement(SyndicationFeed feed, string elementName)
        {
            Collection<XElement> elements =
                feed.ElementExtensions.ReadElementExtensions<XElement>(elementName,
                                                           "http://schemas.zune.net/catalog/music/2007/10");

            return elements.Count > 0 ? elements.First() : null;
        }

        private XElement GetElement(SyndicationItem item, string elementName)
        {
            Collection<XElement> elements =
                item.ElementExtensions.ReadElementExtensions<XElement>(elementName,
                                                           "http://schemas.zune.net/catalog/music/2007/10");

            return elements.Count > 0 ? elements.First() : null;
        }
    }
}
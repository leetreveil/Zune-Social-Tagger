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
            XElement primaryArtistElement =
                item.ElementExtensions.ReadElementExtensions<XElement>("primaryArtist",
                                                           "http://schemas.zune.net/catalog/music/2007/10").First();

            return primaryArtistElement.Elements().Last().Value;
        }

        private Guid GetArtistMediaIDFromTrack(SyndicationItem item)
        {
            XElement primaryArtistElement =
                item.ElementExtensions.ReadElementExtensions<XElement>("primaryArtist",
                                               "http://schemas.zune.net/catalog/music/2007/10").First();

            return primaryArtistElement.Elements().First().Value.ExtractGuidFromUrnUuid();
        }

        private int GetTrackNumberFromTrack(SyndicationItem item)
        {
            XElement trackNumberElement =
                item.ElementExtensions.ReadElementExtensions<XElement>("trackNumber",
                                   "http://schemas.zune.net/catalog/music/2007/10").First();

            return int.Parse(trackNumberElement.Value);
        }

        private string GetAlbumArtist(SyndicationFeed feed)
        {
            Collection<XElement> primaryArtistElements =
                feed.ElementExtensions.ReadElementExtensions<XElement>("primaryArtist",
                                                                       "http://schemas.zune.net/catalog/music/2007/10");


            return primaryArtistElements.Count > 0 ? primaryArtistElements.First().Elements().Last().Value : String.Empty;
        }

        private int? GetReleaseYear(SyndicationFeed feed)
        {
            //TODO: what if the element does not exist?
            Collection<XElement> releaseDateElements =
                feed.ElementExtensions.ReadElementExtensions<XElement>("releaseDate",
                                                                       "http://schemas.zune.net/catalog/music/2007/10");

            return releaseDateElements.Count > 0 ? (int?) DateTime.Parse(releaseDateElements.First().Value).Year : null;
        }

        private string GetArtworkUrl(SyndicationFeed feed)
        {
            XElement imageElement =
             feed.ElementExtensions.ReadElementExtensions<XElement>("image",
                                                                   "http://schemas.zune.net/catalog/music/2007/10").First();

            XElement element = imageElement.Elements().First();

            return String.Format("http://image.catalog.zune.net/v3.0/image/{0}?width=234&height=320",
                                 element.Value.ExtractGuidFromUrnUuid());
        }
    }
}
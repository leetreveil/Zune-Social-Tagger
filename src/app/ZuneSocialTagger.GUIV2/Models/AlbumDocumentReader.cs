using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Syndication;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class AlbumDocumentReader
    {
        private XmlReader _reader;
        private SyndicationFeed _feed;

        public bool Initialize(string url)
        {
            try
            {
                _reader = XmlReader.Create(url);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public AlbumMetaData Read()
        {
            _feed = SyndicationFeed.Load(_reader);

            return _feed != null ? GetAlbumDetails() : null;
        }

        private AlbumMetaData GetAlbumDetails()
        {
            var details = new AlbumMetaData
                              {
                                  AlbumTitle = _feed.Title.Text,
                                  AlbumArtist = GetArtistFromTrack(_feed),
                                  ArtworkUrl = GetArtworkUrl(_feed)
                              };

            return details;
        }

        /// <summary>
        /// This is the primaryArtist for the track, different to the albumArtist
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetArtistFromTrack(SyndicationFeed item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");

            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        private XElement GetElement(SyndicationFeed feed, string elementName)
        {
            Collection<XElement> elements =
                feed.ElementExtensions.ReadElementExtensions<XElement>(elementName, "http://schemas.zune.net/catalog/music/2007/10");

            return elements.Count > 0 ? elements.First() : null;
        }

        private string GetArtworkUrl(SyndicationFeed feed)
        {
            XElement imageElement = GetElement(feed, "image");

            return imageElement != null
                       ? String.Format("{0}{1}?width=234&height=320", "http://image.catalog.zune.net/v3.0/image/",
                                       ExtractGuidFromUrnUuid(imageElement.Elements().First().Value))
                       : null;
        }

        /// <summary>
        /// urn:uuid:c14c4e00-0300-11db-89ca-0019b92a3933
        /// </summary>
        /// <param name="urn"></param>
        /// <returns>c14c4e00-0300-11db-89ca-0019b92a3933</returns>
        public static Guid ExtractGuidFromUrnUuid(string urn)
        {
            return new Guid(urn.Substring(urn.LastIndexOf(':') + 1));
        }

    }
}



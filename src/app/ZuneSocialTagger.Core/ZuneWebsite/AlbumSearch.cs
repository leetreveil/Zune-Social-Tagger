using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearch
    {
        public static IEnumerable<Album> SearchFor(string searchString)
        {
            string searchUrl = String.Format("{0}?q={1}", Urls.Album, searchString);

            return ReadFromXmlDocument(XmlReader.Create(searchUrl));
        }

        public static IEnumerable<Album> GetAlbumsFromArtistGuid(Guid guid)
        {
            var artistAlbumsUrl = String.Format("{0}{1}/albums", Urls.Artist, guid);

            return ReadFromXmlDocument(XmlReader.Create(artistAlbumsUrl));
        }

        public static IEnumerable<Album> ReadFromXmlDocument(XmlReader reader)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    yield return new Album
                         {
                             Title = item.Title.Text,
                             AlbumMediaID = item.Id.ExtractGuidFromUrnUuid(),
                             Artist = GetAlbumArtist(item),
                             ArtworkUrl = GetArtworkUrl(item),
                             ReleaseYear = GetReleaseYear(item)
                         };
                }
            }

        }

        private static string GetAlbumArtist(SyndicationItem feed)
        {
            XElement primaryArtistElement = GetElement(feed, "primaryArtist");

            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        private static string GetReleaseYear(SyndicationItem feed)
        {
            //TODO: refactor this class and AlbumDocumentReader into one single class.
            XElement releaseDateElement = GetElement(feed, "releaseDate");

            return releaseDateElement != null ? DateTime.Parse(releaseDateElement.Value).Year.ToString() :  null;
        }

        private static XElement GetElement(SyndicationItem feed, string elementName)
        {
            Collection<XElement> elements =
                feed.ElementExtensions.ReadElementExtensions<XElement>(elementName, Urls.Schema);

            return elements.Count > 0 ? elements.First() : null;
        }

        private static string GetArtworkUrl(SyndicationItem feed)
        {
            XElement imageElement = GetElement(feed, "image");

            return imageElement != null
                       ? String.Format("{0}{1}?width=60&height=60", Urls.Image,
                                       imageElement.Elements().First().Value.ExtractGuidFromUrnUuid())
                       : null;
        }
    }
}
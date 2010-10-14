using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public static class SyndicationExtensions
    {
        //TODO: refactor all these horrible methods into a base class

        /// <summary>
        /// urn:uuid:c14c4e00-0300-11db-89ca-0019b92a3933
        /// </summary>
        /// <param name="urn"></param>
        /// <returns>c14c4e00-0300-11db-89ca-0019b92a3933</returns>
        public static Guid ExtractGuidFromUrnUuid(this string urn)
        {
            return new Guid(urn.Substring(urn.LastIndexOf(':') + 1));
        }

        public static string GetArtist(this SyndicationItem item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");

            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        public static string GetArtist(this SyndicationFeed item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");

            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        public static IEnumerable<string> GetContributingArtists(this SyndicationItem item)
        {
            XElement contributingArtistsElement = GetElement(item, "contributingArtists");

            return contributingArtistsElement != null
                       ? contributingArtistsElement.Elements().Select(x => x.Elements().Last().Value).ToList()
                       : new List<string>();
        }

        public static string GetGenre(this SyndicationItem item)
        {
            XElement primaryGenreElement = GetElement(item, "primaryGenre");

            return primaryGenreElement != null ? primaryGenreElement.Elements().Last().Value : null;
        }

        public static string GetGenre(this SyndicationFeed item)
        {
            XElement primaryGenreElement = GetElement(item, "primaryGenre");

            return primaryGenreElement != null ? primaryGenreElement.Elements().Last().Value : null;
        }

        public static Guid GetAlbumMediaIdFromTrack(this SyndicationItem item)
        {
            XElement albumElement = GetElement(item, "album");

            return albumElement != null ? albumElement.Elements().First().Value.ExtractGuidFromUrnUuid() : new Guid();
        }

        public static Guid GetArtistMediaIdFromTrack(this SyndicationItem item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");

            return primaryArtistElement != null
                       ? primaryArtistElement.Elements().First().Value.ExtractGuidFromUrnUuid()
                       : new Guid();
        }

        public static string GetTrackNumberFromTrack(this SyndicationItem item)
        {
            XElement trackNumberElement = GetElement(item, "trackNumber");

            return trackNumberElement != null ? trackNumberElement.Value : null;
        }

        public static string GetDiscNumber(this SyndicationItem item)
        {
            XElement discNumberElement = GetElement(item, "discNumber");

            return discNumberElement != null ? discNumberElement.Value : null;
        }

        public static string GetReleaseYear(this SyndicationFeed feed)
        {
            XElement releaseDateElement = GetElement(feed, "releaseDate");

            return GetReleaseDateFromElement(releaseDateElement);
        }

        public static string GetReleaseYear(this SyndicationItem item)
        {
            XElement releaseDateElement = GetElement(item, "releaseDate");

            return GetReleaseDateFromElement(releaseDateElement);
        }

        public static string GetArtworkUrl(this SyndicationFeed feed)
        {
            XElement imageElement = GetElement(feed, "image");

            return GetImageUrlFromElement(imageElement);
        }

        public static string GetArtworkUrl(this SyndicationItem item)
        {
            XElement imageElement = GetElement(item, "image");

            return GetImageUrlFromElement(imageElement);
        }

        private static string GetReleaseDateFromElement(XElement releaseDateElement)
        {
            return releaseDateElement != null ? DateTime.Parse(releaseDateElement.Value).Year.ToString() : null;
        }

        private static string GetImageUrlFromElement(XElement imageElement)
        {
            return imageElement != null
                       ? String.Format("{0}{1}?width=234&height=320", Urls.Image,
                                       imageElement.Elements().First().Value.ExtractGuidFromUrnUuid())
                       : null;
        }

        private static XElement GetElement(SyndicationItem item, string elementName)
        {
            Collection<XElement> elements =
                item.ElementExtensions.ReadElementExtensions<XElement>(elementName, Urls.Schema);

            return elements.Count > 0 ? elements.First() : null;
        }

        private static XElement GetElement(SyndicationFeed feed, string elementName)
        {
            Collection<XElement> elements =
                feed.ElementExtensions.ReadElementExtensions<XElement>(elementName, Urls.Schema);

            return elements.Count > 0 ? elements.First() : null;
        }
    }
}
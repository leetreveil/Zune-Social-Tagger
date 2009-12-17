using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

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

        public IEnumerable<Track> Read()
        {
            SyndicationFeed feed = SyndicationFeed.Load(_reader);

            return feed != null ? GetTracks(feed) : null;
        }

        private IEnumerable<Track> GetTracks(SyndicationFeed feed)
        {
            foreach (var item in feed.Items)
            {
                yield return new Track
                                 {
                                     Title = item.Title.Text,
                                     MediaID = item.Id.ExtractGuidFromUrnUuid(),
                                     AlbumArtist = GetAlbumArtist(feed),
                                     ArtistMediaID = GetArtistMediaIDFromTrack(item),
                                     TrackNumber = GetTrackNumberFromTrack(item),
                                     ContributingArtists = new List<string> { GetArtistFromTrack(item) }.Concat(GetContributingArtists(item)),
                                     Genre = GetGenre(item),
                                     DiscNumber = GetDiscNumber(item),
                                     AlbumName = feed.Title.Text,
                                     Year = GetReleaseYear(feed),
                                     AlbumMediaID = GetAlbumMediaIDFromTrack(item),
                                     ArtworkUrl = GetArtworkUrl(feed)
                                 };

                
            }
        }

        /// <summary>
        /// This is the primaryArtist for the track, different to the albumArtist
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetArtistFromTrack(SyndicationItem item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");

            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        private IEnumerable<string> GetContributingArtists(SyndicationItem item)
        {
            XElement contributingArtistsElement = GetElement(item, "contributingArtists");

            return contributingArtistsElement != null
                       ? contributingArtistsElement.Elements().Select(x => x.Elements().Last().Value).ToList()
                       : new List<string>();
        }

        private string GetGenre(SyndicationItem item)
        {
            XElement primaryGenreElement = GetElement(item, "primaryGenre");

            return primaryGenreElement != null ? primaryGenreElement.Elements().Last().Value : null;
        }

        private Guid GetAlbumMediaIDFromTrack(SyndicationItem item)
        {
            XElement albumElement = GetElement(item, "album");

            return albumElement != null ? albumElement.Elements().First().Value.ExtractGuidFromUrnUuid() : new Guid();
        }

        private Guid GetArtistMediaIDFromTrack(SyndicationItem item)
        {
            XElement primaryArtistElement = GetElement(item, "primaryArtist");

            return primaryArtistElement != null
                       ? primaryArtistElement.Elements().First().Value.ExtractGuidFromUrnUuid()
                       : new Guid();
        }

        private int? GetTrackNumberFromTrack(SyndicationItem item)
        {
            XElement trackNumberElement = GetElement(item, "trackNumber");

            return trackNumberElement != null ? int.Parse(trackNumberElement.Value) : (int?) null;
        }

        private string GetDiscNumber(SyndicationItem item)
        {
            XElement discNumberElement = GetElement(item, "discNumber");

            return discNumberElement != null ? discNumberElement.Value : null;
        }

        private string GetAlbumArtist(SyndicationFeed feed)
        {
            XElement primaryArtistElement = GetElement(feed, "primaryArtist");

            return primaryArtistElement != null ? primaryArtistElement.Elements().Last().Value : null;
        }

        private string GetReleaseYear(SyndicationFeed feed)
        {
            XElement releaseDateElement = GetElement(feed, "releaseDate");

            return releaseDateElement != null ? DateTime.Parse(releaseDateElement.Value).Year.ToString() : null;
        }

        private string GetArtworkUrl(SyndicationFeed feed)
        {
            XElement imageElement = GetElement(feed, "image");

            return imageElement != null
                       ? String.Format("{0}{1}?width=234&height=320", Urls.Image,
                                       imageElement.Elements().First().Value.ExtractGuidFromUrnUuid())
                       : null;
        }

        private XElement GetElement(SyndicationFeed feed, string elementName)
        {
            Collection<XElement> elements =
                feed.ElementExtensions.ReadElementExtensions<XElement>(elementName, Urls.Schema);

            return elements.Count > 0 ? elements.First() : null;
        }

        private XElement GetElement(SyndicationItem item, string elementName)
        {
            Collection<XElement> elements =
                item.ElementExtensions.ReadElementExtensions<XElement>(elementName, Urls.Schema);

            return elements.Count > 0 ? elements.First() : null;
        }
    }
}
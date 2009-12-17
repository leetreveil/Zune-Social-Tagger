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

        public Album Read()
        {
            var album = new Album();

            SyndicationFeed feed = SyndicationFeed.Load(_reader);

            if (feed != null)
            {
                album.Title = feed.Title.Text;
                album.Artist = GetAlbumArtist(feed);
                album.AlbumMediaID = feed.Id.ExtractGuidFromUrnUuid();
                album.ReleaseYear = GetReleaseYear(feed);
                album.ArtworkUrl = GetArtworkUrl(feed);
                album.Tracks = GetTracks(feed.Items,album);
            }

            return album;
        }

        private IEnumerable<Track> GetTracks(IEnumerable<SyndicationItem> items, Album album)
        {
            foreach (var item in items)
            {
                yield return new Track
                                 {
                                     Title = item.Title.Text,
                                     MediaID = item.Id.ExtractGuidFromUrnUuid(),
                                     AlbumArtist = album.Artist,
                                     ArtistMediaID = GetArtistMediaIDFromTrack(item),
                                     TrackNumber = GetTrackNumberFromTrack(item),
                                     ContributingArtists = new List<string> { GetArtistFromTrack(item) }.Concat(GetContributingArtists(item)),
                                     Genre = GetGenre(item),
                                     DiscNumber = GetDiscNumber(item),
                                     AlbumName = album.Title,
                                     Year = album.ReleaseYear.ToString()
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

        private int? GetReleaseYear(SyndicationFeed feed)
        {
            XElement releaseDateElement = GetElement(feed, "releaseDate");

            return releaseDateElement != null ? DateTime.Parse(releaseDateElement.Value).Year : (int?) null;
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
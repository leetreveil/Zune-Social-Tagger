using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Xml;
using System.Diagnostics;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class AlbumSearch
    {
        public static void SearchForAlbumAsync(string searchString, Action<IEnumerable<WebAlbum>> callback)
        {
            ThreadPool.QueueUserWorkItem(_ => callback(SearchForAlbum(searchString).ToList()));
        }

        public static IEnumerable<WebAlbum> SearchForAlbum(string searchString)
        {
            string searchUrl = String.Format("{0}?q={1}", Urls.Album, searchString);

            try
            {
                return ReadFromXmlDocument(XmlReader.Create(searchUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return new List<WebAlbum>();
        }

        public static void SearchForAlbumFromArtistGuidAsync(Guid guid, Action<IEnumerable<WebAlbum>> callback)
        {
            ThreadPool.QueueUserWorkItem(_ => callback(SearchForAlbumFromArtistGuid(guid)));
        }

        public static IEnumerable<WebAlbum> SearchForAlbumFromArtistGuid(Guid guid)
        {
            var artistAlbumsUrl = String.Format("{0}{1}/albums", Urls.Artist, guid);

            try
            {
                return ReadFromXmlDocument(XmlReader.Create(artistAlbumsUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return new List<WebAlbum>();
        }

        private static IEnumerable<WebAlbum> ReadFromXmlDocument(XmlReader reader)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed != null)
            {
                foreach (var item in feed.Items)
                {
                    yield return new WebAlbum
                         {
                             Title = item.Title.Text,
                             AlbumMediaId = item.Id.ExtractGuidFromUrnUuid(),
                             Artist = item.GetArtist(),
                             ArtworkUrl = item.GetArtworkUrl(),
                             ReleaseYear = item.GetReleaseYear(),
                             Genre = item.GetGenre()
                         };
                }
            }
        }
    }
}